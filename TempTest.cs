using System;
using System.IO;
using System.Threading.Tasks;

namespace TempTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var saver = CommunityToolkit.Maui.Storage.FileSaver.Default;
            Console.WriteLine(saver.GetType().Name);
        }
    }
}
