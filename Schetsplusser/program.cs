using System;
using System.Windows.Forms;

static class Program
{
    [STAThreadAttribute]
    static void Main()  //run het programma
    {
        Application.Run(new SchetsEditor());
    }
}