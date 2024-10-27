using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class CoreScript : MonoBehaviour
{
    [SerializeField] private TMP_Text _peonText;
    [SerializeField] private TMP_Text _wheatText;
    [SerializeField] private TMP_Text _foodText;

    [SerializeField] private TMP_Text _orcText;
    [SerializeField] private TMP_Text _orcWaveText;
    [SerializeField] private TMP_Text _warriorText;

    [SerializeField] private TMP_Text _endGameText;


    [SerializeField] private Image _peonImg;
    [SerializeField] private Image _wheatImg;
    [SerializeField] private Image _foodImg;

    [SerializeField] private Image _orcImg;
    [SerializeField] private Image _warriorImg;

    [SerializeField] private Image _peonTimer;
    [SerializeField] private Image _warriorTimer;
    [SerializeField] private Image _orcWaveTimerImg;


    [SerializeField] private Image _loseImg;
    [SerializeField] private Image _winImg;

    [SerializeField] private Image _endGameImg;

    [SerializeField] private GameObject _endGamePanel;
    [SerializeField] private GameObject _orcPanel;

    public AudioSource _audioSource;
    
    [SerializeField] private AudioClip _loseSound;
    [SerializeField] private AudioClip _winSound;
    [SerializeField] private AudioClip _battleSound;
    [SerializeField] private AudioClip _foodHarvestSound;
    [SerializeField] public AudioClip _buttonSound;
    [SerializeField] private AudioClip _restartButtonSound;
    [SerializeField] private AudioClip _warriorHiredSound;
    [SerializeField] private AudioClip _wheatHarvestSound;



    [SerializeField] private int _peonCount;
    [SerializeField] private int _wheatCount;
    [SerializeField] private int _foodCount = 50;

    [SerializeField] private int _peonCost = 2;
    [SerializeField] private int _warriorCost = 3;

    [SerializeField] private int _orcCount;
    [SerializeField] private int _waveCount;
    [SerializeField] private int _orcWaveModifier = 0;
    [SerializeField] private int _warriorCount;

    [SerializeField] private int _wheatModifier = 5;
    [SerializeField] private int _peonConsumptionModifier = 1;
    [SerializeField] private int _warriorConsumptionModifier = 2;



    private bool _orcFlag;
    private bool _winCheck;
    private bool _muteCheck;
    private bool _paused;

    private float _currentTime;
    private float _maxTime;

    private float _wheatGenerateTime;
    private float _maxWheatGenerateTime = 5f;

    private float _hirePeonTime;
    private float _hireWarriorTime;

    private float _foodWasteTime;
    [SerializeField] private float _maxFoodWasteTime = 9f;

    [SerializeField] private float _orcWaveTime = 10f;
    private float _orcImgLifeTime;
    private float _maxOrcWaveTime = 10f;

    [SerializeField] private float _maxHirePeonTime;
    [SerializeField] private float _maxHireWarriorTime;

    [SerializeField] private Button _hirePeonButton;
    [SerializeField] private Button _hireWarriorButton;
    [SerializeField] private Button _muteButton;


    public void HirePeon()
    {
        _audioSource.clip = _buttonSound;
        _audioSource.Play();
        if (_foodCount >= _peonCost)
        {
            _foodCount -= _peonCost;

            _hirePeonTime = _maxHirePeonTime;

            _hirePeonButton.interactable = false;
        }
        else
        {
            _hirePeonButton.interactable = false;

        }

    }

    public void HireWarrior()
    {
        _audioSource.clip = _buttonSound;
        _audioSource.Play();
        if (_foodCount >= _warriorCost)
        {
            _foodCount -= _warriorCost;


            _hireWarriorTime = _maxHireWarriorTime;
            _hireWarriorButton.interactable = false;
        }
        else
        {
            _hireWarriorButton.interactable = false;

        }
    }


    private void WheatTimer()
    {
        _wheatText.text = $" Произведено: {_wheatCount}";

        _wheatGenerateTime -= Time.deltaTime;
        _wheatImg.fillAmount = _wheatGenerateTime / _maxWheatGenerateTime;
        if (_wheatGenerateTime <= 0)
        {
            Debug.Log("WheatTimerActive");

            _wheatGenerateTime = _maxWheatGenerateTime;

            if (_peonCount > 0)
            {
                //_audioSource.clip = _wheatHarvestSound;
                //_audioSource.Play();
                _wheatCount = _peonCount * _wheatModifier;
                _foodCount += _wheatCount;
            }
        }
    }

    private void WasteFood()
    {
        _foodText.text = $" На складах: {_foodCount}";

        _foodWasteTime -= Time.deltaTime;
        _foodImg.fillAmount = _foodWasteTime / _maxFoodWasteTime;
        if (_foodWasteTime <= 0)
        {
            Debug.Log("WasteFood activate");
            _audioSource.clip = _foodHarvestSound;
            _audioSource.Play();
            _foodWasteTime = _maxFoodWasteTime;
            if (_foodCount <= 0)
            {
                _winCheck = false;
                GameOver();
            }
            if (_peonCount > 0 || _warriorCount > 0)
            {

                _foodCount -= (_peonCount * _peonConsumptionModifier) + (_warriorCount * _warriorConsumptionModifier);
            }

        }
        if (_foodCount > 600)
        {
            _winCheck = true;
            GameOver();
        }

    }
    private void PeonTimer()
    {
        _peonText.text = $" Крестьяне: {_peonCount}";
        if (_hirePeonTime > 0)
        {
            _hirePeonTime -= Time.deltaTime;
            _peonTimer.fillAmount = _hirePeonTime / _maxHirePeonTime;
            //Debug.Log(_hirePeonTime);
            if (_hirePeonTime <= 0)
            {
                Debug.Log("PeonHireActive");

                _peonCount++;
                _peonTimer.fillAmount = _maxHirePeonTime;
                _hirePeonButton.interactable = true;
            }
        }
    }
    private void WarriorTimer()
    {
        _warriorText.text = $" Воины: {_warriorCount}";
        if (_hireWarriorTime > 0)
        {
            _hireWarriorTime -= Time.deltaTime;
            _warriorTimer.fillAmount = _hireWarriorTime / _maxHireWarriorTime;
            //Debug.Log(_hireWarriorTime);
            if (_hireWarriorTime <= 0)
            {
                Debug.Log("WarriorHireActive");
                _audioSource.clip = _warriorHiredSound;

                _audioSource.Play();
                _warriorCount++;
                _warriorTimer.fillAmount = _maxHireWarriorTime;
                _hireWarriorButton.interactable = true;
            }
        }
    }
    private void OrcTimer()
    {

        _orcWaveTime -= Time.deltaTime;

        _orcWaveTimerImg.fillAmount = _orcWaveTime / _maxOrcWaveTime;
        //Debug.Log(_hireWarriorTime);
        if (_orcWaveTime <= 0)
        {

            if (_waveCount > 2)
            {
                _orcWaveTimerImg.transform.gameObject.SetActive(false);
                _orcImg.transform.gameObject.SetActive(true);
                Debug.Log("OrcWaveActive");
                _orcCount = _orcWaveModifier;
                _orcFlag = true;
                _audioSource.clip = _battleSound;
                _audioSource.Play();
                _warriorCount -= _orcCount;
                if (_warriorCount < 0)
                {
                    _winCheck = false;
                    GameOver();
                }
                _orcWaveModifier += 3;
                _orcText.text = $" Орков: {_orcWaveModifier}";

                _orcCount = 0;
                _orcWaveTime = _maxOrcWaveTime;

            }
            _orcWaveTime = _maxOrcWaveTime;


        }
        if (_orcWaveTime == _maxOrcWaveTime && Time.timeScale == 1)
        {
            _waveCount++;

            _orcWaveText.text = $" Волна: {_waveCount}";
        }
        if (_orcFlag == true)
        {
            _orcImgLifeTime += Time.deltaTime;
            if (_orcImgLifeTime >= 3f)
            {
                _orcWaveTimerImg.transform.gameObject.SetActive(true);
                _orcImg.transform.gameObject.SetActive(false);
                _orcImgLifeTime = 0;
                _orcFlag = false;
            }

        }


    }



    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        _audioSource = GetComponent<AudioSource>();
      
        _orcWaveText.text = $" Волна: {_waveCount}";
        _orcText.text = $" Орков: {_orcWaveModifier}";
        _waveCount = 0;
        _wheatGenerateTime = _maxWheatGenerateTime;
        _foodWasteTime = _maxFoodWasteTime;
    }

    // Update is called once per frame
    void Update()
    {




        WheatTimer();
        PeonTimer();
        WarriorTimer();
        WasteFood();
        OrcTimer();


    }

    public void RestartScene()
    {
        _audioSource.clip = _restartButtonSound;
        _audioSource.Play();
        SceneManager.LoadScene("SampleScene");
    }
    public void GameOver()
    {
        if (_winCheck == false)
        {
            _audioSource.clip = _loseSound;
            _audioSource.Play();
            _warriorCount = 0;
            Time.timeScale = 0f;
            _endGamePanel.SetActive(true);
            _endGameImg.sprite = _loseImg.sprite;
            _endGameText.text = "Вы проиграли" + "\n" + $"Волн отражено {_waveCount}";
        }
        else
        {
            _audioSource.clip = _winSound;
            _audioSource.Play();
            _endGamePanel.SetActive(true);
            Time.timeScale = 0f;
            _endGameImg.sprite = _winImg.sprite;
            _endGameText.text = "Вы выиграли";
        }



    }
    public void MuteSound()
    {
        _audioSource.clip = _buttonSound;
        _audioSource.Play();
        if (_muteCheck)
        {
            
            _audioSource.mute = true;
        }
        else
        {
           
            _audioSource.mute = false;

        }
        _muteCheck = !_muteCheck;
    }
    public void PauseGame()
    {
        _audioSource.clip = _buttonSound;
        _audioSource.Play();
        if (_paused)
        {
            Time.timeScale = 1.0f;
        }
        else
        {
            Time.timeScale = 0.0f;
        }
        _paused = !_paused;
    }

}
