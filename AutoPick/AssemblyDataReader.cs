namespace AutoPick
{
    using System;
    using System.IO;
    using System.Reflection;

    public class AssemblyDataReader
    {
        private readonly Assembly _assembly;

        public AssemblyDataReader(Assembly assembly)
        {
            _assembly = assembly;
        }

        public Stream Read(string path)
        {
            Stream? stream = _assembly.GetManifestResourceStream(path);

            if (stream == null)
            {
                throw new InvalidOperationException($"Could not find assembly manifest resource at {path}.");
            }

            return stream;
        }
    }
}