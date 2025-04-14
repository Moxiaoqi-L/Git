

using UnityEngine;

public class Constants
{
    // 故事路径
    public static string STORY_PATH = "Assets/Resources/VisualNovel/Story/MainStory/Chapter 1/";
    // 默认故事文件名
    public static string DEFAULT_STORY_FILE_NAME = "Chapter1-0_zh";
    // 文件后缀名
    public static string EXCEL_FILE_EXTENSION = ".xlsx";
    // 默认开始行数
    public static int DEFAULT_START_LINE = 1;

    // 头像图片地址
    public static string AVATAR_PATH = "VisualNovel/Image/Avatar/";
    // 背景图片地址
    public static string BACKGROUND_PATH = "VisualNovel/Image/Background/";   
    // 蒙版图片地址
    public static string MASK_PATH = "VisualNovel/Image/Mask/";    
    // 物品图片地址
    public static string ITEM_PATH = "VisualNovel/Image/Item/";
    // 按钮图片地址
    public static string BUTTON_PATH = "VisualNovel/Image/Button/";
    // 立绘图片地址
    public static string CHARACTER_PATH = "VisualNovel/Image/Character/";
    public static int MAX_LENGTH = 30;

    // 默认打字速度
    public static float DEFAULT_TYPING_SPEED = 0.05f;

    // 自动播放ON
    public static string AUTO_ON = "autoplayon";
    // 自动播放OFF
    public static string AUTO_OFF = "autoplayoff";
    // 默认等待时长
    public static float DEFAULT_AUTO_WAITING_SECONDS = 1.5f;

    // 配音文件地址
    public static string VOCAL_PATH = "VisualNovel/Audio/Vocal/";
    // 背景音乐文件地址
    public static string MUSIC_PATH = "VisualNovel/Audio/Music/";
    // 音效文件地址
    public static string SOUND_PATH = "VisualNovel/Audio/Sound/";

    public static string END_OF_STORY = "END";
    public static string CHOICE = "CHOICE";
    public static string NONE = "none";

    public static string APPEAR_AT = "appearAt";
    public static string DISAPPEAR = "disappear";
    public static string MOVE_TO = "moveTo";
    public static string CHANGE_FACE = "changeFace";
    public static string TURN_AROUND = "turnAround";
    public static float DURATION_TIME = 0.5f;

    public static string COLON = ": ";

    // 颜色点数 RGB
    public static Color REDPOINT = new(0.5f, 0, 0);
    public static Color YELLOWPOINT = new(0.5f, 0.5f, 0);
    public static Color BLUEPOINT = new(0, 0, 0.5f);
}