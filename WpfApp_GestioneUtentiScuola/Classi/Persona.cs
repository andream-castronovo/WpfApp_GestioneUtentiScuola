namespace WpfApp_GestioneUtentiScuola.Classi
{
    public class Persona
    {
        public Persona() { }

        public Persona(string nome)
        {
            Nominativo = nome;
        }

        public string Nominativo { get; set; } = "<nominativo>";

        public override string ToString()
        {
            return $"Nominativo: {Nominativo}";
        }

    }
}
