using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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


                }
            }
            catch (Exception ex)
            {
                Messaggio(ex.Message, "Errore", true);
                return;
            }

            _persone.Insert(++_indicePersone, p);

            AggiornaLabelIndice(lblIndicePersone, _indicePersone);

            Messaggio($"Persona numero {_indicePersone+1} aggiunta","Persona aggiunta");
        }

        private void btnClearList_Click(object sender, RoutedEventArgs e)
        {
            lstOutput.Items.Clear();
        }

        private void btnAggiungiVoto_Click(object sender, RoutedEventArgs e)
        {


            if (txtMateria.Text == "" || txtVoto.Text == "")
            {
                Messaggio("Il campo \"Materia\" o il campo \"Voto\" è vuoto.", "Errore", true);
                return;
            }

            try
            {
                ((Studente)_persone[_indicePersone]).AggiungeValutazione(new Valutazione(txtMateria.Text, double.Parse(txtVoto.Text)));
            }
            catch (Exception ex)
            {
                Messaggio(ex.Message, "Errore", true);
                return;
            }

        }

        private void btnChangePersona_Click(object sender, RoutedEventArgs e)
        {
            if (_indicePersone == -1)
                return;

            switch (((Button)sender).Name)
            {
                case "btnPrimaPersona":
                    _indicePersone = 0;
                    break;
                case "btnPersonaPrecedente":
                    if (_indicePersone != 0)
                        _indicePersone--;
                    break;
                case "btnPersonaSuccessiva":
                    if (_indicePersone != _persone.Count - 1)
                        _indicePersone++;
                    else
                    {
                        PulisciTextBox();
                        return;
                    }
                    break;
                case "btnUltimaPersona":
                    _indicePersone = _persone.Count - 1;
                    break;
            }


            AggiornaLabelIndice(lblIndicePersone, _indicePersone);

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

        void PulisciTextBox()
        {
            TextBox[] txts = { txtLaurea, txtMatricola, txtAnniFuoriCorso, txtNominativo };

            foreach (TextBox t in txts)
                t.Text = "";
        }


        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            PulisciTextBox();
            _indicePersone = _persone.Count - 1;
            AggiornaLabelIndice(lblIndicePersone, _indicePersone);
        }

        void AggiornaLabelIndice(Label lbl, int i)
        {
            string s = "";
            
            if (i >= 0)
                s = $"Indice: {i}";
            else if (i < 0)
                s = $"Indice: No";

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
            Persona p = _persone[i];

            txtNominativo.Text = p.Nominativo;

            if (p is Studente)
            {
                chkStudente.IsChecked = true;
                txtMatricola.Text = ((Studente)p).Matricola + "";


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

            switch (((Button)sender).Name)
            {
                case "btnPrimoVoto":
                    _indiceVoti = 0;
                    break;
                case "btnVotoPrecedente":
                    if (_indiceVoti != 0)
                        _indiceVoti--;
                    break;
                case "btnVotoSuccessivo":
                    if (_indiceVoti != s.NumeroValutazioni - 1)
                        _indiceVoti++;
                    else
                    {
                        txtVoto.Text = "";
                        txtMateria.Text = "";
                        return;
                    }
                    break;
                case "btnUltimoVoto":
                    _indiceVoti = s.NumeroValutazioni - 1;
                    break;
            }

            AggiornaLabelIndice(lblIndiceVoti, _indiceVoti);

            txtVoto.Text = s[_indiceVoti].Voto + "";
            txtMateria.Text = s[_indiceVoti].Materia + "";


        }
    }
}
