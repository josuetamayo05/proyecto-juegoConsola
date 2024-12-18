﻿namespace PruebaGenerarParedes;

using Spectre.Console;

class Program
{
    static string[,] mapa;
    static int puntosJugador1 = 0;
    static int puntosJugador2 = 0;
    static string nombreJugador1; 
    static string nombreJugador2;
    static int[] posicionInicialJugador1 = { 1, 1 }; // Posición inicial del jugador 1
    static int[] posicionInicialJugador2 = { 3, 1 }; // Posición inicial del jugador 2
    static int[] jugador1 = (int[])posicionInicialJugador1.Clone(); // Posición actual del jugador 1
    static int[] jugador2 = (int[])posicionInicialJugador2.Clone(); // Posición actual del jugador 2
    static Random random = new Random(); 


    static void Main(string[] args) // Iniciar el laberinto
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8; // Configurar codificación UTF-8

        MostrarMensajesBienvenida(); // Llamar al método de bienvenida
        MostrarInstrucciones();  // Llamar al método de instrucciones
        
    
        InicializarMapa(15, 15);  // Inicializar el mapa con 12 filas y 12 columnas
        ImprimirMapa();

        while (true)
        {
            MoverJugador(1);           
            if (ComprobarVictoria(jugador1))
            {
                break; // Salir del bucle si el jugador 1 ha ganado
            }

            MoverJugador(2);          
            if (ComprobarVictoria(jugador2))
            {
                break; // Salir del bucle si el jugador 2 ha ganado
            }
        }

        MostrarResultados(); // Mostrar resultados 
    }
    static void MostrarMensajesBienvenida()
    {
        // Crear un panel para mostrar el mensaje de bienvenida
        var panel = new Panel(new Markup(
            "[bold cyan]Bienvenidos al juego del laberinto 👋👾![/]\n" +
            "Presiona cualquier tecla para continuar..."
        ));

        // Configurar el panel
        panel.Border = BoxBorder.Rounded; // Bordes redondeados
        panel.Padding = new Padding(2); // Añadir un poco de espacio interno
        panel.Header = new PanelHeader("[bold yellow]¡Bienvenido![/]"); // Encabezado del panel

        // Renderizar el panel
        AnsiConsole.Render(panel);

        Console.ReadKey(); // Esperar a que el jugador presione una tecla
        Console.Clear(); // Limpiar la consola
    
        nombreJugador1 = AnsiConsole.Ask<string>("Ingrese el nombre del [bold yellow]Jugador 1[/]: ");
        nombreJugador2 = AnsiConsole.Ask<string>("Ingrese el nombre del [bold yellow]Jugador 2[/]: ");
        Console.Clear();
    }

    static void MostrarInstrucciones()
    {
        // Crear un panel para mostrar las instrucciones
        var panel = new Panel(new Markup(
            "[bold yellow]Instrucciones del Juego:[/]\n" +
            "1. Usa las teclas W, A, S, D para mover a tu jugador: 🏃‍♂️\n" +
            "   - W: Arriba ⬆\n" +
            "   - A: Izquierda ⬅\n" +
            "   - S: Abajo ⬇\n" +
            "   - D: Derecha ➡\n" +
            "2. 🚧 Evita los obstáculos marcados con '⛔'.\n" +
            "3. 🎁 Recoge las fichas de recompensa marcadas con '💰' para ganar puntos.\n" +
            "4. 😋 Por cada ficha recompensa que cojas puedes hacer un movimiento extra.\n" +
            "5. 🏁 Llega a la meta marcada con '🚩' para ganar el juego.\n" +
            "¡Buena Suerte! Presiona cualquier tecla para continuar... ⌨"
        ));

        // Configurar el panel
        panel.Border = BoxBorder.Rounded; // Bordes redondeados
        panel.Header = new PanelHeader("[bold cyan]Instrucciones[/]"); // Encabezado del panel

        // Renderizar el panel
        AnsiConsole.Render(panel);

        Console.ReadKey(); // Esperar a que el jugador presione una tecla
        Console.Clear(); // Limpiar la consola
    }
    
    static void InicializarMapa(int filas, int columnas)
    {
        mapa = new string[filas, columnas];

        // Inicializar el mapa con paredes
        for (int i = 0; i < filas; i++)
        {
            for (int j = 0; j < columnas; j++)
            {                                
                mapa[i, j] = "⬜ "; // pared
            }
        }

        GenerarLaberinto(1, 1);
        ColocarFichasYObstaculos(5, 20);

        // Asegurarse de que los jugadores tengan un camino inicial libre
        mapa[1, 1] = "😠";
        mapa[3, 1] = "😎";
        mapa[12, 12] = "🏠 "; // Meta

    }

   

    static void GenerarLaberinto(int fila, int columna)
    {
        // Direcciones posibles: arriba, abajo, izquierda, derecha
        int[] direcciones = { 1, 2, 3, 4 }; // 1: Arriba, 2: Abajo, 3: Izquierda, 4: Derecha
        Shuffle(direcciones); // Mezclar direcciones

        foreach (int direccion in direcciones)
        {
            int nuevaFila = fila;
            int nuevaColumna = columna;

            switch (direccion)
            {
                case 1: nuevaFila -= 2; break; // Arriba
                case 2: nuevaFila += 2; break; // Abajo
                case 3: nuevaColumna -= 2; break; // Izquierda
                case 4: nuevaColumna += 2; break; // Derecha
            }

            // Verificar límites y si la celda está vacía
            if (nuevaFila > 0 && nuevaColumna > 0 && nuevaFila < mapa.GetLength(0) && nuevaColumna < mapa.GetLength(1) && mapa[nuevaFila, nuevaColumna] == "   ")
            {
                // Eliminar la pared entre la celda actual y la nueva celda
                mapa[fila + (nuevaFila - fila) / 2, columna + (nuevaColumna - columna) / 2] = "   ";
                mapa[nuevaFila, nuevaColumna] = "   "; // Marcar la nueva celda como vacía

                // Llamada recursiva
                GenerarLaberinto(nuevaFila, nuevaColumna);
            }
        }
    }

    static void ColocarFichasYObstaculos(int cantidadFichas, int porcentajeObstaculos)
    {
        int filas = mapa.GetLength(0);
        int columnas = mapa.GetLength(1);
        int totalCeldas = (filas - 2) * (columnas - 2); // Total de celdas interiores

        // Calcular el número de obstáculos basados en el porcentaje
        int numeroObstaculos = totalCeldas * porcentajeObstaculos / 100;

        // Colocar fichas de recompensa
        for (int i = 0; i < cantidadFichas; i++)
        {
            int fila, columna;
            do
            {
                fila = random.Next(1, filas - 1);
                columna = random.Next(1, columnas - 1);
            } while (mapa[fila, columna] != "   " || // Asegurarse que el espacio esté vacío
                    (fila == jugador1[0] && columna == jugador1[1]) || // No en la posición del jugador 1
                    (fila == jugador2[0] && columna == jugador2[1])); // No en la posición del jugador 2

            mapa[fila, columna] = "💰 "; // Colocar ficha de recompensa
        }

        // Colocar obstáculos
        for (int i = 0; i < numeroObstaculos; i++)
        {
            int fila, columna;
            do
            {
                fila = random.Next(1, filas - 1);
                columna = random.Next(1, columnas - 1);
            } while (mapa[fila, columna] != "   " || // Asegurarse que el espacio esté vacío
                    (fila == jugador1[0] && columna == jugador1[1]) || // No en la posición del jugador 1
                    (fila == jugador2[0] && columna == jugador2[1])); // No en la posición del jugador 2

            mapa[fila, columna] = "🌳 "; // Colocar obstáculo
        }
    }
    static void Shuffle(int[] array)
    {
        Random rng = new Random();
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            int temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    
    static void ImprimirMapa()
    {
        Console.Clear();
        int filas = mapa.GetLength(0);
        int columnas = mapa.GetLength(1);

        for (int i = 0; i < filas; i++)
        {
            for (int j = 0; j < columnas; j++)
            {
                if (i == jugador1[0] && j == jugador1[1])
                    Console.Write("😠 "); // Representa al jugador 1                   
                else if (i == jugador2[0] && j == jugador2[1])
                    Console.Write("😎 "); // Representa al jugador 2
                else
                Console.Write(mapa[i, j] + ""); // Imprimir cada carácter sin salto de línea
            }
            Console.WriteLine(); // Salto de línea al final de cada fila
        }

        AnsiConsole.MarkupLine($"[bold blue]🎲 Puntos {nombreJugador1} : [/][red]{puntosJugador1}[/] | [bold blue]🎲 Puntos {nombreJugador2} : [/][red]{puntosJugador2}[/]");
        Console.WriteLine();
    }

    static void MostrarResultados()
    {
        var table = new Table();
        table.AddColumn("Jugador");
        table.AddColumn("Puntos");

        table.AddRow(nombreJugador1, puntosJugador1.ToString());
        table.AddRow(nombreJugador2, puntosJugador2.ToString());

        AnsiConsole.Render(table);
        AnsiConsole.MarkupLine("Presiona cualquier tecla para salir...");
        Console.ReadKey(); // Esperar a que el jugador presione una tecla
    }
        
    
    static void MoverJugador(int jugador)
    {
        int filas = mapa.GetLength(0);
        int columnas = mapa.GetLength(1);
        bool movimientoExtra = false; // Controlar si se permite un movimiento extra
        string nombreJugadorActual = jugador == 1 ? nombreJugador1 : nombreJugador2;
        string mensajeRecompensa = "";
        string mensajeTrampa = "";
            
        do
        {
            // Mostrar el mensaje de recompensa si existe
            if (!string.IsNullOrEmpty(mensajeRecompensa))
            {
                Console.WriteLine(mensajeRecompensa);
                mensajeRecompensa = ""; // Limpiar el mensaje de recompensa
            }
            
            if (!string.IsNullOrEmpty(mensajeTrampa))
            {
                Console.WriteLine(mensajeTrampa);
                mensajeTrampa = ""; // Limpiar el mensaje de recompensa
                ImprimirMapa(); // Imprimir el mapa después de mostrar el mensaje de trampa
                break; // Salir del bucle para no seguir pidiendo movimiento
            }    


            AnsiConsole.MarkupLine($"Turno de [bold blue]{nombreJugadorActual}[/] (Jugador {jugador}), ingresa tu movimiento (W/A/S/D) o [bold red]Q[/] para salir: ");
            char movimiento = Console.ReadKey().KeyChar;
            Console.WriteLine(); // Salto de línea después de la entrada

            if (movimiento == 'q' || movimiento == 'Q') // Condición de salida
            {
                AnsiConsole.MarkupLine("[bold white]¡Gracias por jugar![/]");
                Console.ReadLine();
                Environment.Exit(0); // Salir del juego
            }

            int[] posicion = jugador == 1 ? jugador1 : jugador2;
            int nuevaFila = posicion[0];
            int nuevaColumna = posicion[1];

            switch (movimiento)
            {
                case 'w': nuevaFila--; break; // Arriba
                case 's': nuevaFila++; break; // Abajo
                case 'a': nuevaColumna--; break; // Izquierda
                case 'd': nuevaColumna++; break; // Derecha
                default:
                    AnsiConsole.MarkupLine("[bold red]Movimiento no válido. Intenta de nuevo.[/]");
                    continue; // Si la tecla no es válida, volver a pedir movimiento
            }

            // Verificar límites
            if (nuevaFila > 0 && nuevaColumna > 0 && nuevaFila < filas - 1 && nuevaColumna < columnas - 1)
            {
                // Comprobar si ha caído en una trampa
                if (mapa[nuevaFila, nuevaColumna] == "🌳 ")
                {                    
                    // Restaurar la posición inicial
                    if (jugador == 1)
                    {
                        jugador1[0] = posicionInicialJugador1[0]; // Restaurar posición inicial del jugador 1
                        jugador1[1] = posicionInicialJugador1[1];
                    }
                    else
                    {
                        jugador2[0] = posicionInicialJugador2[0]; // Restaurar posición inicial del jugador 2
                        jugador2[1] = posicionInicialJugador2[1];
                    }
                    mensajeTrampa = $"{nombreJugadorActual}, has caído en una trampa. Vuelves a tu posición inicial."; // Mensaje de trampa
                    // No imprimir el mapa aquí para que el mensaje se vea claramente
                    break; // Salir del método para evitar más movimientos
                }

                // Verificar si puede moverse a un nuevo espacio
                if (mapa[nuevaFila, nuevaColumna] != "🌳 ") // No puede moverse a un obstáculo
                {
                    // Actualizar posición del jugador
                    posicion[0] = nuevaFila;
                    posicion[1] = nuevaColumna;
                
                    // Comprobar si ha recogido una recompensa
                    Console.WriteLine("Evaluando posicion");
                    if (mapa[nuevaFila, nuevaColumna] == "💰 ")
                    {
                        mapa[nuevaFila, nuevaColumna] = "   "; // limpiar la posición

                        if (jugador == 1)
                        {                            
                            puntosJugador1++;     
                            mensajeRecompensa = $"¡{nombreJugador1} has recogido una ficha de recompensa! Puedes realizar otro movimiento extra. Puntos: {puntosJugador1}";                  
                        }
                        else
                        {
                            puntosJugador2++;            
                            mensajeRecompensa = $"¡{nombreJugador2} has recogido una ficha de recompensa! Puedes realizar otro movimiento extra. Puntos: {puntosJugador2}";                
                        }
                        movimientoExtra = true;
                    }
                    else
                    {
                        movimientoExtra = false;
                    }

                    ImprimirMapa();
                    
                }                                                       
                else 
                {
                    AnsiConsole.MarkupLine("[bold red]¡No puedes moverte allí! Hay un obstáculo.[/]");
                    break; // Salir del bucle si se sale de los límites
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]¡Te has salido de los límites![/]");
                break; // Salir del bucle si se sale de los límites
            }    
        
        } while (movimientoExtra); // Continuar si se ha recogido una ficha y no se ha caído en una trampa 
    }
            
    static bool ComprobarVictoria(int[] jugador)
    {
        // Definir la posición de la meta
        int metaFila = 12;
        int metaColumna = 12;

        // Comprobar si la posición del jugador coincide con la posición de la meta
        if (jugador[0] == metaFila && jugador[1] == metaColumna)
        {
            string nombreJugadorActual = jugador == jugador1 ? nombreJugador1 : nombreJugador2;
            AnsiConsole.MarkupLine($"[bold blue]{nombreJugadorActual} ha alcanzado la meta y ha ganado el juego! 🎉[/]");
            return true; // El jugador ha ganado
        }

        return false; // El jugador no ha ganado
    }   
}