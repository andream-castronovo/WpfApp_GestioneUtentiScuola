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
            _indicePersone = 0;
            _indiceVoti = 0;
        }

        private void DisabilitaControllo(Control fe)
        {
            fe.IsEnabled = false;
            fe.Visibility = Visibility.Collapsed;

            if (fe is TextBox)
            {
                TextBox t = fe as TextBox;
                string lblName = t.Name.Replace("txt", "lbl");
                
                Label lbl = (Label) FindName(lblName);
                lbl.Visibility = Visibility.Collapsed;
            }

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

            // Colore di default
            // fe.BackgroundColor = Brushes.LightGray;
            // fe.BorderBrush = Brushes.DarkGrey;
        }
        private void Messaggio(string mex, string title, bool error = false)
        {
            MessageBox.Show(mex, title, MessageBoxButton.OK, error ? MessageBoxImage.Error : MessageBoxImage.Information);
        }





        #region Metodi controlli

        #region Click
        private void btnMostraTutto_Click(object sender, RoutedEventArgs e)
        {
            bool almenoUno = false;
            foreach (Persona persona in _persone)
            {
                if (persona != null)
                {
                    lstOutput.Items.Add(persona); // .ToString() implicito nel metodo Add()
                    almenoUno = true;

                    if (persona is Studente)
                    {
                        Studente s = (Studente)persona;

                        for (int k = 0; k < s.NumeroValutazioni; k++)
                        {
                            lstOutput.Items.Add(s[k]);
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

            lstOutput.Items.Add(
                $"Voto massimo tra i massimi degli studenti: {votoMaxMax}" +
                "\n" +
                $"Voto minimo tra i minimi degli studenti: {votoMinMin}" +
                "\n" +
                $"Voto Medio tra i voti medi degli studenti: {sommaVotiMedi / nStudenti:f2}"
                );

        }

        private void btnNuovo_Click(object sender, RoutedEventArgs e)
        {
            Persona p = null;

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
                p = new Studente(
                    txtNominativo.Text,
                    int.Parse(txtMatricola.Text),
                    null
                    );
            else if ((bool)chkStudenteFuoriCorso.IsChecked)
                p = new StudenteFuoriCorso(
                    txtNominativo.Text,
                    int.Parse(txtMateria.Text),
                    null,
                    int.Parse(txtAnniFuoriCorso.Text)
                    );

            _persone.Add(p);
        }

        private void btnClearList_Click(object sender, RoutedEventArgs e)
        {
            lstOutput.Items.Clear();
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
                    AbilitaControllo(grpVoti);
                    AbilitaControllo(txtMatricola);
                    break;
                case "chkStudenteFuoriCorso":
                    AbilitaControllo(grpVoti);
                    AbilitaControllo(txtMatricola);
                    AbilitaControllo(txtAnniFuoriCorso);
                    break;
            }
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
    }
}
