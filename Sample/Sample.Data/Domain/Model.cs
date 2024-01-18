using System;

namespace CiccioSoft.VirtualList.Sample.Domain
{
    public class Model
    {
        public Model(uint numero, string name)
        {
            Numero = numero;
            Name = name;
        }

        public virtual uint Id
        {
            get; set;
        }
        public virtual uint Numero
        {
            get; set;
        }
        public virtual string? Name
        {
            get; set;
        }

        public override bool Equals(object? obj)
        {
            return obj is Model model &&
                   Numero == model.Numero;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Numero);
        }
    }
}
