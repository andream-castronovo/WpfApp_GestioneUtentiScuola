using System;

namespace WpfApp_GestioneUtentiScuola.Classi
{
    public class Persona
    {
        private string _nominativo;
        
        public Persona() { }

        public Persona(string nome)
        {
            Nominativo = nome;
        }

        public string Nominativo
        {
            get
            {
                return _nominativo;
            }
            set
            {
                if (value == "" || value == null)
                    _nominativo = "<inserire nome>";
                else
                    _nominativo = value;
            }
        }

        public override string ToString()
        {
            return $"Nominativo: {Nominativo}";
        }

        public virtual string OttieniStringaBackup()
        {
            return $"Persona#{Nominativo}";
        }
    }
}
