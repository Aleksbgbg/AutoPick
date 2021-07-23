namespace AutoPick
{
    public class Champion
    {
        public Champion(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}