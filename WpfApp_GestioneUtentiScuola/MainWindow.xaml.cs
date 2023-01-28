using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WpfApp_GestioneUtentiScuola.Classi;

namespace WpfApp_GestioneUtentiScuola
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<Persona> _persone;
        int _indicePersone;
        int _indiceVoti;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisabilitaControllo(txtAnniFuoriCorso);
            DisabilitaControllo(txtLaurea);
            DisabilitaControllo(txtMatricola);
            DisabilitaControllo(grpVoti);

            _persone = new List<Persona>();
            _indicePersone = -1;
            _indiceVoti = -1;
            chkPersona.IsChecked = true;

            AggiornaLabelIndicePersone(lblIndicePersone, _indicePersone);
            AggiornaLabelIndicePersone(lblIndiceVoti, _indiceVoti);

            //LeggiDaFile();
        }

        #region Metodi
        private void DisabilitaControllo(Control fe)
        {
            fe.IsEnabled = false;
            fe.Visibility = Visibility.Collapsed;

            if (fe is TextBox)
            {
                TextBox t = fe as TextBox;
                string lblName = t.Name.Replace("txt", "lbl");

                Label lbl = (Label)FindName(lblName);
                lbl.Visibility = Visibility.Collapsed;
            }

            else if (fe is Label)
                ((Label)fe).Visibility = Visibility.Collapsed;


            // Se volessi cambiare anche il colore di bordi o background
            // fe.BackgroundColor = Brushes.Red;
            // fe.BorderBrush = Brushes.Red;
        }
        private void AbilitaControllo(Control fe)
        {
            fe.IsEnabled = true;
            fe.Visibility = Visibility.Visible;

            if (fe is TextBox)
            {
                TextBox t = fe as TextBox;
                string lblName = t.Name.Replace("txt", "lbl");

                Label lbl = (Label)FindName(lblName);
                lbl.Visibility = Visibility.Visible;
            }

            else if (fe is Label)
                ((Label)fe).Visibility = Visibility.Visible;

            // Colore di default
            // fe.BackgroundColor = Brushes.LightGray;
            // fe.BorderBrush = Brushes.DarkGrey;
        }
        private void Messaggio(string mex, string title, bool error = false)
        {
            MessageBox.Show(mex, title, MessageBoxButton.OK, error ? MessageBoxImage.Error : MessageBoxImage.Information);
        }
        #endregion




        #region Metodi controlli

        #region Click
        private void btnMostraTutto_Click(object sender, RoutedEventArgs e)
        {
            bool almenoUno = false;
            int i = 0;
            foreach (Persona persona in _persone)
            {
                if (persona != null)
                {
                    lstOutput.Items.Add("Persona N°" + (++i));
                    lstOutput.Items.Add("\t" + persona); // .ToString() implicito nel metodo Add()
                    almenoUno = true;

                    if (persona is Studente)
                    {
                        Studente s = (Studente)persona;

                        lstOutput.Items.Add("\tValutazioni:");
                        for (int k = 0; k < s.NumeroValutazioni; k++)
                        {
                            lstOutput.Items.Add("\t\t" + s[k]);
                        }

                    }
                }
            }

            if (!almenoUno)
                Messaggio(
                    "Non è stata aggiunta alcuna persona.",
                    "Niente da mostrare"
                    );

        }

        private void btnStatisticheComplessive_Click(object sender, RoutedEventArgs e)
        {
            lstOutput.Items.Clear();

            double votoMaxMax = 0;
            double votoMinMin = 10;
            double sommaVotiMedi = 0;

            int nStudenti = 0;

            foreach (Persona persona in _persone)
            {
                double vMax, vMin;

                if (persona != null && persona is Studente)
                {
                    sommaVotiMedi += ((Studente)persona).CalcoliConVoti(out vMax, out vMin);

                    if (vMax > votoMaxMax)
                        votoMaxMax = vMax;
                    if (vMin < votoMinMin)
                        votoMinMin = vMin;

                    nStudenti++;
                }
            }

            if (nStudenti != 0)
                lstOutput.Items.Add(
                    $"Voto massimo tra i massimi degli studenti: {votoMaxMax}" +
                    "\n" +
                    $"Voto minimo tra i minimi degli studenti: {votoMinMin}" +
                    "\n" +
                    $"Voto Medio tra i voti medi degli studenti: {sommaVotiMedi / nStudenti:f2}"
                    );
            else
                Messaggio("Non ci sono studenti nella lista", "Nessuna statistica");

        }

        private void btnNuovaPersona_Click(object sender, RoutedEventArgs e)
        {
            Persona p = null;
            try
            {
                if ((bool)chkPersona.IsChecked)
                    p = new Persona(
                        txtNominativo.Text
                        );
                else if ((bool)chkDocente.IsChecked)
                    p = new Docente(
                        txtNominativo.Text,
                        txtLaurea.Text
                        );
                else if ((bool)chkStudente.IsChecked)
                {
                    p = new Studente(
                        txtNominativo.Text,
                        int.Parse(txtMatricola.Text),
                        null
                        );
                    grpVoti.IsEnabled = true;
                    DisabilitaControllo(btnModificaVoto);
                }
                else if ((bool)chkStudenteFuoriCorso.IsChecked)
                {
                    p = new StudenteFuoriCorso(
                        txtNominativo.Text,
                        int.Parse(txtMatricola.Text),
                        null,
                        int.Parse(txtAnniFuoriCorso.Text)
                        );
                
                    grpVoti.IsEnabled = true;
                    DisabilitaControllo(btnModificaVoto);

                }
            }
            catch (Exception ex)
            {
                Messaggio(ex.Message, "Errore", true);
                return;
            }

            if (_indicePersone == _persone.Count)
                _persone.Add(p);
            else if (_indicePersone >= -1)
                _persone.Insert(++_indicePersone, p);

            AggiornaLabelIndicePersone(lblIndicePersone, _indicePersone);

            Messaggio($"Persona numero {_indicePersone} aggiunta","Persona aggiunta");
        }

        private void btnClearList_Click(object sender, RoutedEventArgs e)
        {
            lstOutput.Items.Clear();
        }

        private void btnAggiungiVoto_Click(object sender, RoutedEventArgs e)
        {
            Studente s = ((Studente)_persone[_indicePersone]);
            if (txtMateria.Text == "" || txtVoto.Text == "")
            {
                Messaggio("Il campo \"Materia\" o il campo \"Voto\" è vuoto.", "Errore", true);
                return;
            }

            try
            {
                s.AggiungeValutazione(new Valutazione(txtMateria.Text, double.Parse(txtVoto.Text)));
            }
            catch (Exception ex)
            {
                Messaggio(ex.Message, "Errore", true);
                return;
            }
            
            _indiceVoti = s.NumeroValutazioni - 1;
            AggiornaLabelIndiceVoti(lblIndiceVoti, _indiceVoti);
            Messaggio("Valutazione aggiunta con successo!", "Valutazione aggiunta");
        }

        private void btnChangePersona_Click(object sender, RoutedEventArgs e)
        {
            if (_persone.Count == 0)
            {
                Messaggio("Nessuna persona trovata.","No persone");
                return;
            }

            switch (((Button)sender).Name)
            {
                case "btnPrimaPersona":
                    _indicePersone = 0;
                    break;
                case "btnPersonaPrecedente":
                    if (_indicePersone >= 0)
                        _indicePersone--;
                    break;
                case "btnPersonaSuccessiva":
                    if (_indicePersone < _persone.Count)
                        _indicePersone++;
                    break;
                case "btnUltimaPersona":
                    _indicePersone = _persone.Count - 1;
                    break;
            }

            
            AggiornaLabelIndicePersone(lblIndicePersone, _indicePersone);

            if (_indicePersone == _persone.Count || _indicePersone == -1)
            {
                PulisciTextBox();
                DisabilitaControllo(grpVoti);
                DisabilitaControllo(btnModifica);
                return;
            }

            AbilitaControllo(btnModifica);
            CaricaPersonaAIndice(_indicePersone);
        }
        #endregion
        
        #region Checked e Unchecked
        private void SceltaRuolo_Checked(object sender, RoutedEventArgs e)
        {
            switch (((RadioButton)sender).Name)
            {
                case "chkDocente":
                    AbilitaControllo(txtLaurea);
                    break;
                case "chkStudente":
                    grpVoti.Visibility = Visibility.Visible;
                    AbilitaControllo(txtMatricola);
                    break;
                case "chkStudenteFuoriCorso":
                    grpVoti.Visibility = Visibility.Visible;
                    AbilitaControllo(txtMatricola);
                    AbilitaControllo(txtAnniFuoriCorso);
                    break;
            }

        }

        void PulisciTextBox(bool voti = false)
        {
            TextBox[] txts = null;
            
            if (!voti)
                txts = new TextBox[] { txtLaurea, txtMatricola, txtAnniFuoriCorso, txtNominativo };
            else
                txts = new TextBox[] { txtVoto, txtMateria };

            foreach (TextBox t in txts)
                t.Text = "";
        }


        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            PulisciTextBox();
            AggiornaLabelIndicePersone(lblIndicePersone, _indicePersone);
        }

        void AggiornaLabelIndicePersone(Label lbl, int i)
        {
            string s = "";
            
            if (i >= 0 && i != _persone.Count)
                s = $"Indice: {i}";
            else if (i < 0)
                s = $"Indice: -1\n(Nessuna persona selezionata)";
            else if (i == _persone.Count)
                s = $"Indice: {i}\n(Indice ancora non esistente)";
            
            lbl.Content = s;
        }
        void AggiornaLabelIndiceVoti(Label lbl, int i)
        {
            string s = "";

            if (i >= 0 && i != ((Studente)_persone[_indicePersone]).NumeroValutazioni)
                s = $"Indice: {i}";
            else if (i < 0)
                s = $"Indice: -1\n(Nessuna valutazione selezionata)";
            else if (i == ((Studente)_persone[_indicePersone]).NumeroValutazioni)
                s = $"Indice: {i}\n(Indice ancora non esistente)";

            lbl.Content = s;
        }

        private void SceltaRuolo_Unchecked(object sender, RoutedEventArgs e)
        {
            switch (((RadioButton)sender).Name)
            {
                case "chkDocente":
                    DisabilitaControllo(txtLaurea);
                    break;
                case "chkStudente":
                    DisabilitaControllo(grpVoti);

                    DisabilitaControllo(txtMatricola);
                    break;
                case "chkStudenteFuoriCorso":
                    DisabilitaControllo(grpVoti);
                    DisabilitaControllo(txtMatricola);
                    DisabilitaControllo(txtAnniFuoriCorso);
                    break;
            }
        }
        
        #endregion

        #endregion
        
        private void CaricaPersonaAIndice(int i)
        {
            if (i < 0 || i >= _persone.Count)
                return;

            Persona p = _persone[i];

            txtNominativo.Text = p.Nominativo;
            chkPersona.IsChecked = true;

            if (p is Studente)
            {
                chkStudente.IsChecked = true;
                txtMatricola.Text = ((Studente)p).Matricola + "";
                _indiceVoti = 0;

                grpVoti.IsEnabled = true;
            }

            if (p is StudenteFuoriCorso)
            {
                chkStudenteFuoriCorso.IsChecked = true;
                txtAnniFuoriCorso.Text = ((StudenteFuoriCorso)p).AnniFuoriCorso + "";
                
                grpVoti.IsEnabled = true;
            }

            if (p is Docente)
            {
                chkDocente.IsChecked = true;
                txtLaurea.Text = ((Docente)p).Laurea;
            }
        }

        private void btnModifica_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Nessun controllo. Tutti hanno nominativo.
                _persone[_indicePersone].Nominativo = txtNominativo.Text;
                
                // Utilizzo IS e non .GetType() == typeof(...)
                // Questo perché con IS posso controllare anche se è un figlio della classe scelta
                // Per esempio:
                //      StudenteFuoriCorso è figlio di Studente
                //      Se il rif. nella lista in posizione _indicePersone
                //      è di tipo StudenteFuoriCorso, entrerà anche nell'IF dello Studente
                //      e per questo saranno assegnati sia Matricola sia AnniFuoriCorso.
                
                if (_persone[_indicePersone] is Studente)
                    ((Studente)_persone[_indicePersone]).Matricola = int.Parse(txtMatricola.Text);
                if (_persone[_indicePersone] is StudenteFuoriCorso)
                    ((StudenteFuoriCorso)_persone[_indicePersone]).AnniFuoriCorso = int.Parse(txtAnniFuoriCorso.Text);

                if (_persone[_indicePersone] is Docente)
                    ((Docente)_persone[_indicePersone]).Laurea = txtLaurea.Text;

            }
            catch (Exception ex)
            {
                Messaggio(ex.Message, "Errore", true);
                return;
            }

            Messaggio($"Persona numero {_indicePersone + 1} modificata", "Persona modificata");
        }

        private void ChangeVoto_Click(object sender, RoutedEventArgs e)
        {
            // Qui sicuramente abbiamo uno studente
            
            Studente s = (Studente)_persone[_indicePersone];

            if (s.NumeroValutazioni == 0)
            {
                Messaggio("Nessuna valutazione trovata.", "No valutazioni");
                return;
            }

            switch (((Button)sender).Name)
            {
                case "btnPrimoVoto":
                    _indiceVoti = 0;
                    break;
                case "btnVotoPrecedente":
                    if (_indiceVoti >= 0)
                        _indiceVoti--;
                    break;
                case "btnVotoSuccessivo":
                    if (_indiceVoti < s.NumeroValutazioni)
                        _indiceVoti++;
                    break;
                case "btnUltimoVoto":
                    _indiceVoti = s.NumeroValutazioni - 1;
                    break;
            }

            AggiornaLabelIndiceVoti(lblIndiceVoti, _indiceVoti);
            
            if (_indiceVoti >= s.NumeroValutazioni || _indiceVoti < 0)
            {
                PulisciTextBox(voti: true);
                DisabilitaControllo(btnModificaVoto);
                return;
            }

            AbilitaControllo(btnModificaVoto);

            txtVoto.Text = s[_indiceVoti].Voto + "";
            txtMateria.Text = s[_indiceVoti].Materia + "";
        }

        private void btnModificaVoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((Studente)_persone[_indicePersone])[_indiceVoti].Materia = txtMateria.Text;
                ((Studente)_persone[_indicePersone])[_indiceVoti].Voto = double.Parse(txtVoto.Text);
                Messaggio("Valutazione modificata con successo!", "Modifica valutazione");
            }
            catch (Exception ex)
            {
                Messaggio(ex.Message, "Errore", true);
            }
        }
        
        private void btnSaveAll_Click(object sender, RoutedEventArgs e)
        {
            SalvataggioSuFile();
            Messaggio("Salvato", "Sheesh");
        }

        private void SalvataggioSuFile()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Messaggio(ex.Message, "Errore", true);
            }

            //StreamWriter sw = new StreamWriter("../../Salvataggio.txt");

            //foreach (Persona p in _persone)
            //{
            //    sw.WriteLine(p.OttieniStringaBackup());    
            //}

            //sw.Close();
        }

        private void LeggiDaFile()
        {

            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Messaggio(ex.Message, "Errore", true);
                return;
            }

            StreamReader sr = new StreamReader("../../Salvataggio.txt");

            List<string> rows = new List<string>();

            while (!sr.EndOfStream)
            {
                rows.Add(sr.ReadLine());
            }

            sr.Close();
            
            foreach (string row in rows)
            {
                string[] dati = row.Split('#');

                Persona p = null;
                switch (dati[0])
                {
                    case "Persona":
                        p = new Persona(dati[1]);
                        break;
                    case "Docente":
                        p = new Docente(dati[1], dati[2]);
                        break;
                    case "Studente":
                        break;
                    case "StudenteFuoriCorso":
                        break;
                }

                Console.Write(p);

            }

        }
    
    
    }
}
