using TicketPremium.Console.Utils;

namespace TicketPremium.Console.Servicios
{
    public static class ConsoleUI
    {
        public static void Escribir(string texto, ConsoleColor color)
        {
            var prev = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.Write(texto);
            System.Console.ForegroundColor = prev;
        }

        public static void EscribirLinea(string texto, ConsoleColor color)
        {
            var prev = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.WriteLine(texto);
            System.Console.ForegroundColor = prev;
        }

        public static void DibujarSulliDerecha()
        {
            var ancho = System.Console.WindowWidth;
            if (ancho < 50) ancho = 50;
            var inicioX = Math.Max(ancho - 88, 0);
            var colorAnterior = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Magenta;

            var lineas = new[]
            {
                "    .              .                         .    .                   ",
                "                     .    .     ..              .    . .         .    ",
                "       .     .                  ===:...                               ",
                " .                             #.-:::+=..              .         .    ",
                "              .             . -*+--:.   --:..                       . ",
                "        .   .                  @*++--=*=*+===-:.                      ",
                "=-::----:-:-.  .             .=@%#+%#*****+*===---.       .           ",
                "%*+-------+*##*#*=-:.  .: .=#%%%@%@%%####**++*==*====--:::.           ",
                "%##+--=--=*###%@@##***%@%%#%%%#%%%%@%%%%%%#%%%*++=-=***++=:           ",
                "###*+=----+=+*%#*+**########*++=-====**%%*+*%****=*+++*#%%+        .  ",
                "%%+==-----==+******##########**+----:-#: -.=--=-=#%#*+=**:+        .  ",
                "***+-------++=*#**###########**+=----:::#@@..+==####*++=+%#. .        ",
                "**#+=+---=-===*#######%######**+------::.=.  :=**##*#+=*==-.          ",
                "+++==----=-=+**####%#######*##+==-------::-----@@=.##*#+==-.          ",
                "===----++=-=+*########*+++==*#%------:-##+----=*@= .#%#%*+:   .       ",
                "=++==++==+**#%%%#%#***++===+%=@#=------+@@*=--::   ###:   .          .",
                "=+==++*#+++*%%%######*+====+%+@*+*---::-:%@*=---::--:             .   ",
                "++*++***#%%%%%#####**+==+==+#**@@-%=--:::::##%@*:::-.     ..          ",
                "++*+**##%%%%%%#####****+===+*#-:  -#%------:::::::--                  ",
                "*****+*#%%%%%#######***+=--=+*%- .-@.=%+:-:::::::--:.                 ",
                "*+**#*#%%%%%%######**#+++=--=+*#--: +@+=%*:::::::--              .    ",
                "+***#%%%%%%%%#######**#*=+=---==+=---.-@@#*%=-----              .     ",
                "*####%%%%%%%%########*#*++-------=+=------=.*----.                    ",
                "*###%%#%%%%%%%#########**++---------==----------:    .                ",
                "*##%%%%%%%%%%%%%%#######**++-=-----------------:             .      . ",
                "###%%%%%%%%%%%%%%%#%###***+=+=----------------.                        ",
                "###%%%%%%%%%%%%%%%%#######**+==--------------.                      . ",
                "####%%%%%%%%%%%%%%%%%%###**#*++=------------.  .     .   .   .    . . ",
                "#%%%##%%%%%%%%%%%%%%%%%%###**+++==--------: .            .            ",
                "####%%%##%%%%%%%%%%%%%%%%%###***+++==--:.                             ",
                "#######%%%#%%%%%%%%%%%%%%%####*#**++=::.                  .           ",
                "*#####%%%%%%%%%%%%%#######**+++=-:--------*+:                         ",
                "%%%####*##%#%%########*+===----:-----:-=+*-                            ",
                "@@%%%%%#**=+++*++++#%#*=-------------::::=:...         .           .  ",
                "%%%@@%+*==+====++#@@%%%#===-----------::::=-.. .. .          .    ..  ",
                "@@%@%*++*++*+++*#@@%%@%%*++-=-------------=-:.........             .  ",
                "@%#%#*+*#**+*###%@@%@@%%*+==-------:--==--    .... .   .      ....    ",
                "++***###+***++*%@@@%@@%%*==-----------:::. .  .....:+*:       .       ",
                "**#=***+#****##@%@@@@@#*+=--------==---:::      ...:===        .      ",
                "#++++*+**#*#+**#%%%%%#*++==+------===---::      ...:---               ",
                "+=**+**+***##**####****==+++=-------+*==-. .   ........        .      ",
                "*###***+*##****+***++=+++===+---------***:   ....:....                ",
                "%@#****#*#**#****++++*+++=+++=--------===.....:.::...                 ",
                "@%###*+**+**#*#*#*++**#*+-:. #*+=-----+-...::::.....                  ",
                "@%@@*+*##*****#**+***-  .   .=#*++==:.=-......:::     .        .      ",
                "@%%#*##%%#%###%##*=.   . .         .                                  ",
                "%@%%%%%%%%%%%%%%%=  .                          ..                .    ",
                "%%######%%##%%%#-          . .      .             .   .               ",
                "*********######*     .     .                                          ",
                "+++++++++******+      .  .                                        ..  ",
                "-=-=====+++++*+                                                        ",
                "--------===+++.                     ..                  .       .     ",
                "--:-------==+                                .                        ",
                "--:--:------..                          .        .                    ",
                "-::--:-----..    .        .                  .                        ",
                "-:--:-::--.                                  .                        ",
                "-::-------                       .              .                     ",
                "-:-----:--:       .                    .     ..        .              ",
                "-:---------:    .   .                      . .      .                 ",
                "-------=-==-                      .      .                            ",
                "----=+*%%##*:                              .                          ",
                "======#%%##**.                                 .    .          .      ",
                "=+=====*%#***=.                          .                            ",
            };

            for (var i = 0; i < lineas.Length; i++)
            {
                var y = i;
                if (y >= System.Console.WindowHeight) break;
                System.Console.SetCursorPosition(inicioX, y);
                var linea = lineas[i];
                var maxLen = ancho - inicioX;
                if (linea.Length > maxLen)
                    linea = linea.Substring(0, maxLen);
                System.Console.Write(linea);
            }

            System.Console.ForegroundColor = colorAnterior;
            System.Console.SetCursorPosition(0, 0);
        }

        public static void MostrarEncabezado()
        {
            System.Console.Clear();
            DibujarSulliDerecha();
            System.Console.SetCursorPosition(0, 0);
            System.Console.WriteLine();
            EscribirLinea("  ================================================", ConsoleColor.Blue);
            Escribir("  |", ConsoleColor.Blue);
            System.Console.Write("             ");
            Escribir("TICKET PREMIUM MONSTER    ", ConsoleColor.Cyan);
            System.Console.Write("       ");
            EscribirLinea("|", ConsoleColor.Blue);
            Escribir("  |", ConsoleColor.Blue);
            System.Console.Write("        ");
            Escribir("Boletos para partidos de futbol    ", ConsoleColor.DarkCyan);
            System.Console.Write("   ");
            EscribirLinea("|", ConsoleColor.Blue);
            EscribirLinea("  ================================================", ConsoleColor.Blue);
            System.Console.WriteLine();
        }

        public static void MostrarEncabezadoSinLimpiar()
        {
            var (left, top) = System.Console.GetCursorPosition();
            DibujarSulliDerecha();
            System.Console.SetCursorPosition(0, 0);
            System.Console.WriteLine();
            EscribirLinea("  ================================================", ConsoleColor.Blue);
            Escribir("  |", ConsoleColor.Blue);
            System.Console.Write("             ");
            Escribir("TICKET PREMIUM MONSTER    ", ConsoleColor.Cyan);
            System.Console.Write("       ");
            EscribirLinea("|", ConsoleColor.Blue);
            Escribir("  |", ConsoleColor.Blue);
            System.Console.Write("        ");
            Escribir("Boletos para partidos de futbol    ", ConsoleColor.DarkCyan);
            System.Console.Write("   ");
            EscribirLinea("|", ConsoleColor.Blue);
            EscribirLinea("  ================================================", ConsoleColor.Blue);
            System.Console.WriteLine();
        }

        public static int LeerOpcion(int min, int max)
        {
            while (true)
            {
                System.Console.SetCursorPosition(0, Math.Max(0, System.Console.CursorTop));
                System.Console.Write("  Seleccione una opcion: ");
                var input = System.Console.ReadLine()?.Trim();
                System.Console.WriteLine();
                if (int.TryParse(input, out var opcion) && opcion >= min && opcion <= max)
                    return opcion;

                var cursorTop = System.Console.CursorTop;
                EscribirLinea($"  Opcion invalida. Ingrese entre {min} y {max}.", ConsoleColor.Red);
                System.Threading.Thread.Sleep(1000);
                if (System.Console.CursorTop >= 2)
                {
                    System.Console.SetCursorPosition(0, System.Console.CursorTop - 2);
                    ClearCurrentLine();
                    System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
                    ClearCurrentLine();
                }
            }
        }

        public static string LeerTexto(string mensaje, bool obligatorio = true)
        {
            while (true)
            {
                System.Console.Write(mensaje);
                var input = System.Console.ReadLine()?.Trim() ?? "";
                if (!obligatorio || !string.IsNullOrEmpty(input))
                    return input;

                var cursorTop = System.Console.CursorTop;
                EscribirLinea("  Este campo es obligatorio.", ConsoleColor.Red);
                System.Threading.Thread.Sleep(1000);
                if (System.Console.CursorTop >= 2)
                {
                    System.Console.SetCursorPosition(0, System.Console.CursorTop - 2);
                    ClearCurrentLine();
                    System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
                    ClearCurrentLine();
                }
            }
        }

        public static string LeerCedula()
        {
            while (true)
            {
                System.Console.Write("  Ingrese su cedula (10 digitos): ");
                var input = System.Console.ReadLine()?.Trim() ?? "";
                System.Console.WriteLine();

                if (!CedulaValidator.EsValida(input, out var error))
                {
                    EscribirLinea($"  {error}", ConsoleColor.Red);
                    System.Threading.Thread.Sleep(1200);
                    var cursorTop = System.Console.CursorTop;
                    if (System.Console.CursorTop >= 2)
                    {
                        System.Console.SetCursorPosition(0, System.Console.CursorTop - 2);
                        ClearCurrentLine();
                        System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
                        ClearCurrentLine();
                    }
                    continue;
                }

                return input;
            }
        }

        public static void ClearCurrentLine()
        {
            var currentLineCursor = System.Console.CursorTop;
            System.Console.SetCursorPosition(0, currentLineCursor);
            System.Console.Write(new string(' ', Math.Max(System.Console.WindowWidth - 1, 20)));
            System.Console.SetCursorPosition(0, currentLineCursor);
        }

        public static void Pausa(string mensaje = "Presione ENTER para continuar...")
        {
            System.Console.WriteLine();
            System.Console.Write($"  {mensaje}");
            try
            {
                System.Console.ReadLine();
            }
            catch (InvalidOperationException)
            {
                // Input is piped — continue without waiting
            }
        }
    }
}
