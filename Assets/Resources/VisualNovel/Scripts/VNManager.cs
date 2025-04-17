using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VNManager : MonoBehaviour
{
    public static VNManager Get = null;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI speakerContent;
    public TypewriterEffect typewriterEffect;
    public GameObject historyScrollView; 

    private AudioSource vocalAudio;
    private AudioSource backgroundMusic;
    private AudioSource soundAudio;

    public Image avatarImage;
    public Image backgroundImage;
    public Image characterImage1;
    public Image characterImage2;
    public Image characterImage3;
    public Image maskImage;
    public Image itemImage;

    public GameObject choicePanel;
    public Button choiceButton1;
    public Button choiceButton2;
    public Button choiceButton3;
    public Button choiceButton4;

    public GameObject topButtons;
    public Button autoButton;
    public Button skipButton;
    public Button historyButton;

    private readonly string storyPath = Constants.STORY_PATH;
    private readonly string defaultStoryFileName = Constants.DEFAULT_STORY_FILE_NAME;
    private readonly string excelFileExtension = Constants.EXCEL_FILE_EXTENSION;
    private List<ExcelReader.ExcelData> storyData;
    private LinkedList<string> historyRecords = new LinkedList<string>();

    private int currentLine;
    private string currentStoryFileName;

    private bool isAutoPlay = false;
    private bool isSkip = false;

    private void Awake() {
        if (Get == null)
        {
            Get = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        topButtonsAddListener();
        GetAudioSource();
        InitializeAndLoadStory(defaultStoryFileName);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsHittingTopButtons() && !historyScrollView.activeSelf)
            {
                DisplayNextLine();
            }
        }
    }

    // 获取音频
    private void GetAudioSource()
    {
        vocalAudio = GameObject.Find("Vocal").GetComponent<AudioSource>();
        backgroundMusic = GameObject.Find("BGM").GetComponent<AudioSource>();
        soundAudio = GameObject.Find("Sound").GetComponent<AudioSource>();   
        Debug.Log(vocalAudio);    
        Debug.Log(backgroundMusic);   
        Debug.Log(soundAudio);    
    }

    void RecordHistory(string speaker, string content)
    {
        string historyNameRecord = speaker;
        string historyRecord = content;
        if (historyRecords.Count >= Constants.MAX_LENGTH)
        {
            historyRecords.RemoveFirst(); 
        }
        historyRecords.AddLast(historyRecord);
    }
    void topButtonsAddListener()
    {
        autoButton.onClick.AddListener(OnAutoButtonClick);
        skipButton.onClick.AddListener(OnSkipButtonClick);
        historyButton.onClick.AddListener(OnHistoryButtonClick);
    }
    void InitializeAndLoadStory(string fileName)
    {
        Initialize();
        LoadStoryFromFile(fileName);
        DisplayNextLine();
    }
    public void Initialize()
    {
        currentLine = Constants.DEFAULT_START_LINE;

        avatarImage.gameObject.SetActive(false);
        characterImage1.gameObject.SetActive(false);
        characterImage2.gameObject.SetActive(false);
        characterImage2.gameObject.SetActive(false);
        choicePanel.SetActive(false);
    }
    public void LoadStoryFromFile(string fileName)
    {
        currentStoryFileName = fileName;
        storyData = ExcelReader.ReadExcel(currentStoryFileName);
        if (storyData == null || storyData.Count == 0)
        {
            // DEBUG
        }
    }
    void DisplayNextLine()
    {
        if (currentLine >= storyData.Count - 1)
        {
            if (isAutoPlay)
            {
                isAutoPlay = false;
                UpdateButtonImage(Constants.AUTO_OFF, autoButton);
            }
            if (storyData[currentLine].speakerName == Constants.END_OF_STORY)
            {
                Debug.Log(Constants.END_OF_STORY);
            }
            if (storyData[currentLine].speakerName == Constants.CHOICE)
            {
                ShowChoices();
            }
            return;
        }
        if (typewriterEffect.IsTyping())
        {
            typewriterEffect.CompleteLine();
        }
        else
        {
            DisplayThisLine();
        }
    }
    void DisplayThisLine()
    {
        var data = storyData[currentLine];
        speakerName.text = data.speakerName;
        speakerContent.text = data.speakerContent;
        typewriterEffect.StartTyping(speakerContent.text);

        RecordHistory(speakerName.text, speakerContent.text);

        if (NotNullNorEmpty(data.avatarImageFileName))
        {
            UpdateAvatarImage(data.avatarImageFileName);
        }
        else
        {
            avatarImage.gameObject.SetActive(false);
        }
        if (NotNullNorEmpty(data.vocalAudioFileName))
        {
            PlayVocalAudio(data.vocalAudioFileName);
        }
        if (NotNullNorEmpty(data.backgroundFileName))
        {
            UpdateBackgroundImage(data.backgroundFileName);
        }
        if (NotNullNorEmpty(data.maskFileName))
        {
            UpdateMaskImage(data.maskFileName);
        }
        if (NotNullNorEmpty(data.itemFileName))
        {
            UpdateItemImage(data.itemFileName);
        }
        if (NotNullNorEmpty(data.backgroundMusicFileName))
        {
            PlayBackgroundMusic(data.backgroundMusicFileName);
        }
        if (NotNullNorEmpty(data.soundAudioFileName))
        {
            PlaySoundAudio(data.soundAudioFileName);
        }
        if (NotNullNorEmpty(data.character1Action))
        {
            UpdateCharacterImage(data.character1Action, data.character1ImageFileName,
                                characterImage1);
        }
        if (NotNullNorEmpty(data.character2Action))
        {
            UpdateCharacterImage(data.character2Action, data.character2ImageFileName,
                                characterImage2);
        }
        if (NotNullNorEmpty(data.character3Action))
        {
            UpdateCharacterImage(data.character3Action, data.character3ImageFileName,
                                characterImage3);
        }
        currentLine++;
    }
    bool NotNullNorEmpty(string str)
    {
        return !string.IsNullOrEmpty(str);
    }
    void ShowChoices()
    {
        var data = storyData[currentLine];
        choiceButton1.onClick.RemoveAllListeners();
        choiceButton2.onClick.RemoveAllListeners();
        choicePanel.SetActive(true);
        choiceButton1.GetComponentInChildren<TextMeshProUGUI>().text = data.speakerContent;
        choiceButton1.onClick.AddListener(() => InitializeAndLoadStory(data.avatarImageFileName));
        choiceButton2.GetComponentInChildren<TextMeshProUGUI>().text = data.backgroundFileName;
        choiceButton2.onClick.AddListener(() => InitializeAndLoadStory(data.maskFileName));
        choiceButton3.GetComponentInChildren<TextMeshProUGUI>().text = data.itemFileName;
        choiceButton3.onClick.AddListener(() => InitializeAndLoadStory(data.vocalAudioFileName));
        choiceButton4.GetComponentInChildren<TextMeshProUGUI>().text = data.soundAudioFileName;
        choiceButton4.onClick.AddListener(() => InitializeAndLoadStory(data.backgroundMusicFileName));
    }
    void PlayVocalAudio(string audioFileName)
    {
        string audioPath = Constants.VOCAL_PATH + audioFileName;
        PlayAudio(audioPath, vocalAudio, false);
    }
    void PlayBackgroundMusic(string musicFileName)
    {
        string musicPath = Constants.MUSIC_PATH + musicFileName;
        PlayAudio(musicPath, backgroundMusic, true);
    }
    void PlaySoundAudio(string musicFileName)
    {
        string musicPath = Constants.SOUND_PATH + musicFileName;
        PlayAudio(musicPath, soundAudio, false);
    }
    void UpdateAvatarImage(string imageFileName)
    {
        var imagePath = Constants.AVATAR_PATH + imageFileName;
        UpdateImage(imagePath, avatarImage);
    }
    void UpdateBackgroundImage(string imageFileName)
    {
        string imagePath = Constants.BACKGROUND_PATH + imageFileName;
        backgroundImage.DOFade(0, Constants.DURATION_TIME).OnComplete(() => UpdateImage(imagePath, backgroundImage, true));
    }
    void UpdateMaskImage(string imageFileName)
    {
        string imagePath = Constants.MASK_PATH + imageFileName;
        UpdateImage(imagePath, maskImage, true);
    }
    void UpdateItemImage(string imageFileName)
    {
        string imagePath = Constants.ITEM_PATH + imageFileName;
        UpdateImage(imagePath, itemImage, true);
    }
    void UpdateCharacterImage(string action, string imageFileName, Image characterImage)
    {
        if (action.StartsWith(Constants.APPEAR_AT))
        {
            string imagePath = Constants.CHARACTER_PATH + imageFileName;
            var coordinates = action.Substring(9, action.Length - 10).Split(',');
            float x = float.Parse(coordinates[0]);
            float y = float.Parse(coordinates[1]);
            UpdateImage(imagePath, characterImage);
            var newPosition = new Vector2(x, y);
            characterImage.rectTransform.anchoredPosition = newPosition;
            characterImage.DOFade(1, Constants.DURATION_TIME).From(0);

        }
        else if (action.StartsWith(Constants.MOVE_TO))
        {
            var coordinates = action.Substring(7, action.Length - 8).Split(',');
            float x = float.Parse(coordinates[0]);
            float y = float.Parse(coordinates[1]);            
            characterImage.rectTransform.DOAnchorPos(new Vector2(x, y), Constants.DURATION_TIME);
        }
        else if (action == Constants.DISAPPEAR)
        {
            characterImage.DOFade(0, Constants.DURATION_TIME).OnComplete(() => characterImage.gameObject.SetActive(false));
        }
        else if (action == Constants.CHANGE_FACE)
        {
            string imagePath = Constants.CHARACTER_PATH + imageFileName;
            UpdateImage(imagePath, characterImage);
        }
        else if (action == Constants.TURN_AROUND)
        {
            characterImage.rectTransform.DOScaleX(-characterImage.rectTransform.localScale.x, 0.2f);
        }
    }
    void UpdateImage(string imagePath, Image image, bool isFade = false)
    {
        Sprite sprite = Resources.Load<Sprite>(imagePath);
        if (sprite != null)
        {
            image.sprite = sprite;
            image.gameObject.SetActive(true);
            if(isFade){
                image.DOFade(1, Constants.DURATION_TIME);
            }
        }else
        {
            image.DOFade(0, Constants.DURATION_TIME);
        }
    }
    void PlayAudio(string audioPath, AudioSource audioSource, bool isLoop)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(audioPath);
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            audioSource.loop = isLoop;
        }
        else
        {
            audioSource.DOFade(0, 2f);
        }
    }
    void StopAllAudio()
    {
        vocalAudio.Stop();
        backgroundMusic.Stop();
        soundAudio.Stop();
    }
    bool IsHittingTopButtons()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            topButtons.GetComponent<RectTransform>(),
            Input.mousePosition,
            null
        );
    }
    void OnAutoButtonClick()
    {
        isAutoPlay = !isAutoPlay;
        UpdateButtonImage((isAutoPlay ? Constants.AUTO_ON : Constants.AUTO_OFF), autoButton);
        if (isAutoPlay)
        {
            StartCoroutine(StartAutoPlay());
        }
    }
    void OnSkipButtonClick()
    {
        // 弹出确认退出？
        SceneLoaderWithAnimation sceneLoaderWithAnimation = FindObjectOfType<SceneLoaderWithAnimation>();
        sceneLoaderWithAnimation.LoadScene("Chapter 0");
        StopAllAudio();
    }
    void OnHistoryButtonClick()
    {
        HistoryManager.Instance.ShowHistory(historyRecords);
    }
    void UpdateButtonImage(string imageFileName, Button button)
    {
        string imagePath = Constants.BUTTON_PATH + imageFileName;
        UpdateImage(imagePath, button.image);
    }
    private IEnumerator StartAutoPlay()
    {
        while (isAutoPlay)
        {
            if (!typewriterEffect.IsTyping())
            {
                DisplayNextLine();
            }
            yield return new WaitForSeconds(Constants.DEFAULT_AUTO_WAITING_SECONDS);
        }
    }
}
