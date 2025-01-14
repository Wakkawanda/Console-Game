namespace Brodilka.SettingGame;

public class SettingGame
{
    public static char Border = '#';
    public static char Player = '@';
    public static char Bullet = '-';
    public static char Enemy = '?';
    
    public static short RenderSecond = 1;
    
    public static byte JumpHeight = 2;
    public static short JumpUpgradeSecond = 500; 
    public static bool JumpStatus = true;
    
    public static short GravityUpgradeSecond = 500;
    public static bool GravityStatus = true;

    public static byte BulletRange = 15;
    public static short BulletUpgradeSecond = 200;
    public static bool BulletStatus = true;

    public static byte EnemyUpgradeSecond = 10;
}