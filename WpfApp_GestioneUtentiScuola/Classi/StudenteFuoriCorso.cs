namespace WpfApp_GestioneUtentiScuola.Classi
{
    public class StudenteFuoriCorso : Studente
    {
        public int AnniFuoriCorso { get; set; }

        public StudenteFuoriCorso(
            string nome, 
            int matricola,
            Valutazione[] valutazioni,
            int anni) : base (nome, matricola, valutazioni)
        {
            AnniFuoriCorso = anni;
        }

        public override string ToString()
        {
            return base.ToString() + $", Anni fuori corso: {AnniFuoriCorso}" ;
        }
    }
}
