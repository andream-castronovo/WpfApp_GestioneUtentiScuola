namespace WpfApp_GestioneUtentiScuola.Classi
{
    public class Valutazione
    {
        public Valutazione() { }
        public Valutazione(string m, double v)
        {
            Materia = m;
            Voto = v;
        }
        
        // TODO: Aggiungere i controlli nei set
        public string Materia { get; set; }
        public double Voto { get; set; }

        public void IncrementoPercentuale(double p)
        {
            Voto += Voto * p / 100;
        }

        public override string ToString()
        {
            return $"Materia: {Materia}, Voto: {Voto}";
        }


    }
}
