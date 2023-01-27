using System;
using System.Collections.Generic; // Per List<T>

namespace WpfApp_GestioneUtentiScuola.Classi
{
    public class Studente : Persona
    {
        public int Matricola { get; set; }

        private List<Valutazione> _valutazioni;

        public Valutazione this[int i]
        {
            get
            {
                ControllaIndice(i);
                return _valutazioni[i];
            }
            set
            {
                ControllaIndice(i);
                _valutazioni[i] = value;
            }
        }

        private void ControllaIndice(int i)
        {
            if (i < 0 || i >= _valutazioni.Count)
            {
                throw new ArgumentException($"L'indice deve essere compreso tra 0 e {_valutazioni.Count - 1}", "Valutazione");
            }
        }

        public int NumeroValutazioni
        {
            get
            {
                return _valutazioni.Count;
            }
        }


        public Studente()
        {
            _valutazioni = new List<Valutazione>();
        }

        public Studente(string n, int m, Valutazione[] v) : base (n)
        {
            Matricola = m;

            if (v == null)
                _valutazioni = new List<Valutazione>();
            else
                _valutazioni = new List<Valutazione>(v);
        }

        public void AggiungeValutazione(Valutazione val)
        {
            // TODO: Controllo del null. Perché aggiungere una valutazione se tanto è vuota.
            _valutazioni.Add(val);
        }

        public double CalcoliConVoti(out double vMax, out double vMin)
        {
            double somma = 0;
            vMax = 0;
            vMin = 10;

            for (int i = 0; i < _valutazioni.Count; i++)
            {
                somma += _valutazioni[i].Voto;

                if (_valutazioni[i].Voto > vMax)
                    vMax = _valutazioni[i].Voto;

                if (_valutazioni[i].Voto < vMin)
                    vMax = _valutazioni[i].Voto;
            }

            return somma / _valutazioni.Count;
        }

        public override string ToString()
        {
            return base.ToString() + $", Matricola: {Matricola}";

            // TODO: Altro??? ☺
        }

    }
}
