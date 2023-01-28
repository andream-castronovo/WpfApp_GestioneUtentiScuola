using System;

namespace WpfApp_GestioneUtentiScuola.Classi
{
    public class Docente : Persona
    {
        private string _laurea;
        
        public string Laurea
        {
            get 
            {
                return _laurea;
            }
            set
            {
                if (value == null || value.Length == 0)
                    throw new ArgumentException("Fornire una stringa effettiva!", "Laurea");
                
                _laurea = value;
            }
        }

        public Docente() { } 

        public Docente(string nome, string laurea) : base (nome)
        {
            Laurea = laurea;
        }

        public override string ToString()
        {
            return base.ToString() + $", Laureato in: {Laurea}";
        }

        public override string OttieniStringaBackup()
        {
            return $"Docente#{Nominativo}#{Laurea}";
        }
    }
}
