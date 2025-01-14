using static Brodilka.SettingGame.SettingGame;

namespace Brodilka
{
    internal class Program
    {
        private static CancellationTokenSource? _cancellationTokenSource;

        public static async Task Main(string[] args)
        {
            char[,] map = new char[10, 90];
            
            byte[] playerPosition = { (byte)(map.GetLength(0) - 2), 1 };
            byte[] bulletPosition = {0,0};
            
            bool programStatus = true;
            

            _cancellationTokenSource = new CancellationTokenSource();

            SetBorders(map, playerPosition);
            var renderTask = Render(map, _cancellationTokenSource.Token);
            
            while (programStatus)
            {
                PlayerController(ref programStatus, map, playerPosition, bulletPosition);
                await GravityUpgrade(map, playerPosition);
            }

            _cancellationTokenSource.Cancel();
            await renderTask;
        }

        public static void PlayerController(ref bool status, char[,] map, byte[] playerPosition, byte[] bulletPosition)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        JumpPlayer(map, playerPosition, JumpHeight, 0);
                        break;
                    
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        break;
                    
                    case ConsoleKey.A:
                        if (BulletStatus)
                        {
                            bulletPosition[1] = (byte)(playerPosition[1] - 1);
                        }
                        
                        ShotUpdate(map, playerPosition,  bulletPosition, 1);
                        break;
                    
                    case ConsoleKey.LeftArrow:
                        MovePlayer(map, playerPosition, 0, -1);
                        break;
                    
                    case ConsoleKey.D:
                        if (BulletStatus)
                        {
                            bulletPosition[1] = (byte)(playerPosition[1] + 1);
                        }
                        
                        ShotUpdate(map, playerPosition,  bulletPosition, 2);
                        break;
                    
                    case ConsoleKey.RightArrow:
                        MovePlayer(map, playerPosition, 0, 1);
                        break;
                    
                    case ConsoleKey.L:
                        Console.WriteLine("Выход");
                        status = false;
                        break;
                }
            }
        }
        private static async Task JumpPlayer(char[,] map, byte[] playerPosition, int deltaX, int deltaY)
        {
            if (JumpStatus)
            {
                JumpStatus = false;
                GravityStatus = false;
                int newX;
                int newY;
            
                for (int i = 0; i < deltaX; i++)
                {
                    newX = playerPosition[0] - 1;
                    newY = playerPosition[1] + deltaY;
                
                    if (map[newX, newY] != Border)
                    {
                        map[playerPosition[0], playerPosition[1]] = ' ';
                        map[newX, playerPosition[1]] = Player;
                        playerPosition[0] = (byte)newX;
                        playerPosition[1] = (byte)newY;
                        
                    } else break;
                    await Task.Delay(JumpUpgradeSecond);
                }
            
                for (int i = 0; i < deltaX; i++)
                {
                    newX = playerPosition[0] + 1;
                    newY = playerPosition[1] + deltaY;
                    
                    if (map[newX, newY] != Border)
                    {
                        map[newX, playerPosition[1]] = Player;
                        map[playerPosition[0], playerPosition[1]] = ' ';
                        playerPosition[0] = (byte)newX;
                        playerPosition[1] = (byte)newY;
                        
                    }else break;
                    await Task.Delay(JumpUpgradeSecond);
                }
                JumpStatus = true;
                GravityStatus = true;
            }
        }
        
        private static void MovePlayer(char[,] map, byte[] playerPosition, int deltaX, int deltaY)
        {
            int newX = playerPosition[0] + deltaX;
            int newY = playerPosition[1] + deltaY;

            if (map[newX, newY] != Border)
            {
                map[newX, newY] = Player;
                map[playerPosition[0], playerPosition[1]] = ' ';
                playerPosition[0] = (byte)newX;
                playerPosition[1] = (byte)newY;
            }
        }

        private static async Task ShotUpdate(char[,] map, byte[] playerPosition, byte[] bulletPosition, int choice)
        {
            if (BulletStatus)
            {
                bulletPosition[0] = playerPosition[0];
                BulletStatus = false;
                if (choice == 1)
                {
                    for (int i = 0; i < BulletRange; i++)
                    {
                        if (map[bulletPosition[0], bulletPosition[1] - 1] != Border)
                        {
                            if (i > 0)
                            {
                                map[bulletPosition[0], bulletPosition[1] + 1] = ' ';
                            }
                        
                            map[bulletPosition[0], bulletPosition[1]] = Bullet;
                            bulletPosition[1] -= 1;
                        
                            await Task.Delay(BulletUpgradeSecond);
                        }else break;
                    }
                
                    map[bulletPosition[0], bulletPosition[1] + 1] = ' ';   
                }else
                {
                    for (int i = 0; i < BulletRange; i++)
                    {
                        if (map[bulletPosition[0], bulletPosition[1] + 1] != Border)
                        {
                            if (i > 0)
                            {
                                map[bulletPosition[0], bulletPosition[1] - 1] = ' ';
                            }
                        
                            map[bulletPosition[0], bulletPosition[1]] = Bullet;
                            bulletPosition[1] += 1;
                        
                            await Task.Delay(BulletUpgradeSecond);
                        }else break;
                    }
                
                    map[bulletPosition[0], bulletPosition[1] - 1] = ' ';
                }
                
                BulletStatus = true;
            }
        }
        
        private static async Task GravityUpgrade(char[,] map, byte[] playerPosition)
        {
            int newX = playerPosition[0] + 1;
            int newY = playerPosition[1];

            if (map[newX, newY] != Border && GravityStatus)
            {
                map[newX, newY] = Player;
                map[playerPosition[0], playerPosition[1]] = ' ';
                playerPosition[0] = (byte)newX;
                await Task.Delay(GravityUpgradeSecond);
            }
        }
        
        public static void SetBorders(char[,] map, byte[] playerPosition)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (i == 0 || i == map.GetLength(0) - 1 || j == 0 || j == map.GetLength(1) - 1)
                        map[i, j] = Border;
                    else
                        map[i, j] = ' ';
                }
            }

            FillingMap(map);
            map[playerPosition[0], playerPosition [1]] = Player;
        }

        public static void FillingMap(char[,] map)
        {
            map[7, 7] = Border;
            map[6, 8] = Border;
            map[5, 9] = Border;
            map[5, 10] = Border;
            map[5, 11] = Border;
            map[5, 12] = Border;
            map[6, 13] = Border;
            map[7, 14] = Border;
            
            map[5, 18] = Border;
            map[5, 19] = Border;
            map[5, 20] = Border;
            map[5, 21] = Border;
            map[5, 22] = Border;
            map[5, 23] = Border;
            map[5, 24] = Border;
            
            map[4, 29] = Border;
            map[4, 30] = Border;
            map[4, 31] = Border;
            map[4, 32] = Border;
            map[4, 33] = Border;
            map[4, 34] = Border;

            map[6, 29] = Border;
            map[6, 30] = Border;
            map[6, 31] = Border;
            map[6, 32] = Border;
            map[6, 33] = Border;
            map[6, 34] = Border;
            
            map[8, 40] = Enemy;
        }

        public static async Task Render(char[,] map, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Console.Clear();
                
                Console.WriteLine("Передвижение [<] [>] \nПрыжок [^] \nСтрелять  [A] [D]");
                
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        Console.Write(map[i, j]);
                    }
                    Console.WriteLine();
                }

                await Task.Delay(RenderSecond, token);
            }
        }
    }
}