using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Quizgame : BaseGame
{
    [SerializeField]
    private GameUI UI;
    public static string TriviaContainer = "trivia"; // REQUIRED-FIELD : Azure Blob Container for trivia questions
    public static List<TriviaQuestion> Questions;
    public static TriviaQuestion CurrentQuwestion;
    [Header("Question UI")]
    public Text QuestionText;
    public Image AnswerABck;
    public Text AnswerAText;
    public Image AnswerBBck;
    public Text AnswerBText;
    public Image AnswerCBck;
    public Text AnswerCText;
    public Image AnswerDBck;
    public Text AnswerDText;
    public Image TimerRadial;
    public GameObject Curtain;
    public Image ResultImg;
    public Sprite correctImg;
    public Sprite incorrectImg;
    public Sprite TimesUpImg;
    public Text TimerText;
    public float qTime = 90f;
    private bool TimesUp = false;
    public float gameOverTime = 6f;
    private float GOTimer = 0f;
    public Color FullTimeCol = Color.green;
    public Color NoTimeCol = Color.red;
    private int fails = 0;

    public void Init()
    {
        Questions = new List<TriviaQuestion>(GameManager.Azure.GetQuestionsByPartitionKeyContains(TriviaContainer, TriviaContainer));
    }

    public void NewQuestion(TriviaQuestion question)
    {
        Curtain.SetActive(false);
        CurrentQuwestion = question;
        QuestionText.text = question.Question;
        AnswerABck.color = Color.white;
        AnswerAText.text = question.AnswerA;
        AnswerBBck.color = Color.white;
        AnswerBText.text = question.AnswerB;
        AnswerCBck.color = Color.white;
        AnswerCText.text = question.AnswerC;
        AnswerDBck.color = Color.white;
        AnswerDText.text = question.AnswerD;
        Timer = qTime;
        GOTimer = gameOverTime;
        TimerText.text = Timer.ToString("c0");
        gameObject.SetActive(true);
        fails = 0;
        TimesUp = false;
        SetState(State.Playing);
    }

    void Update ()
    {
        switch (Cur_State)
        {
            case State.Splash:

                break;
            case State.Menu:

                break;
            case State.Playing:
                Timer -= Time.deltaTime;
                if(Timer <= 0)
                {
                    TimesUp = true;
                    SetState(State.Gameover);
                }
                TimerText.text = Timer.ToString("00");
                TimerRadial.fillAmount = Timer / qTime;
                TimerRadial.color = Color.Lerp(NoTimeCol, FullTimeCol, TimerRadial.fillAmount);
                break;
            case State.Paused:

                break;
            case State.Gameover:
                GOTimer -= Time.deltaTime;
                if(GOTimer <= 0)
                {
                    SetState(State.Menu);
                    gameObject.SetActive(false);
                }
                break;
        }
    }

    public override void EnterState(State state)
    {
        switch (state)
        {
            case State.Splash:
                //UI.Splash_PNL.SetActive(true);
                break;
            case State.Menu:
                //UI.Menu_PNL.SetActive(true);
                break;
            case State.Playing:
                UI.Playing_PNL.SetActive(true);

                break;
            case State.Paused:
                UI.Paused_PNL.SetActive(true);
                break;
            case State.Gameover:
                //UI.Gameover_PNL.SetActive(true);
                Curtain.SetActive(true);
                if (fails == 0 && !TimesUp)
                {
                    ResultImg.sprite = correctImg;
                    SoundManager.Instance.PlaySound("Quiz_Right", SFX);
                    UserUtilities.AllocatePoints(CurrentQuwestion.Value);
                    UIManager.Instance.IndicateScore(CurrentQuwestion.Value, true);
                    UserUtilities.Save();
                }
                else if(TimesUp)
                {
                    ResultImg.sprite = TimesUpImg;
                    SoundManager.Instance.PlaySound("Quiz_GameOver", SFX);
                }
                else
                {
                    ResultImg.sprite = incorrectImg;
                    SoundManager.Instance.PlaySound("Quiz_Wrong", SFX);
                }
                break;
        }
    }

    public override void ExitState(State state)
    {
        switch(state)
        {
            case State.Splash:
                //UI.Splash_PNL.SetActive(false);
                break;
            case State.Menu:
                //UI.Menu_PNL.SetActive(false);
                break;
            case State.Playing:
                //UI.Playing_PNL.SetActive(false);
                break;
            case State.Paused:
                //UI.Paused_PNL.SetActive(false);
                break;
            case State.Gameover:
                //UI.Gameover_PNL.SetActive(false);
                break;
        }
    }

    public override void SetState(State newState)
    {
        ExitState(Cur_State);
        base.SetState(newState);
        EnterState(newState);
    }

    public override void CloseGame()
    {

    }

    /// <summary> Searches the questions for ones from the proper place </summary>
    public void CheckForTrivia(string label)
    {
        List<TriviaQuestion> validQuestions = new List<TriviaQuestion>();
        System.Random rand = new System.Random((int)DateTime.Now.Ticks);

        for (int i = 0; i < Questions.Count; ++i)
        {
            if (label == Questions[i].Location)
            {
                validQuestions.Add(Questions[i]);
            }
        }

        if (validQuestions.Count >= 1)
        {
            int r = rand.Next(0, validQuestions.Count);
            NewQuestion(validQuestions[r]);
        }
    }

    /// <summary> Checks to see if the answer selected is correct </summary>
    private bool CompareAnswer(string selectedAnswer)
    {
        if(selectedAnswer == CurrentQuwestion.CorrectAnswer)
        {
            return true;
        }
        return false;
    }

    /// <summary> Selects Answer </summary>
    public void AnswerClick(string question)
    {
        SoundManager.Instance.PlaySound("HRCA_button");
        bool answer = false;
        switch (question)
        {
            case "A":
                answer = CompareAnswer(AnswerAText.text);
                AnswerABck.color = (answer) ?(FullTimeCol):(NoTimeCol);
                break;
            case "B":
                answer = CompareAnswer(AnswerBText.text);
                AnswerBBck.color = (answer) ? (FullTimeCol) : (NoTimeCol);
                break;
            case "C":
                answer = CompareAnswer(AnswerCText.text);
                AnswerCBck.color = (answer) ? (FullTimeCol) : (NoTimeCol);
                break;
            case "D":
                answer = CompareAnswer(AnswerDText.text);
                AnswerDBck.color = (answer) ? (FullTimeCol) : (NoTimeCol);
                break;
        }
        if(!answer)
        {
            fails++;
        }
        SetState(State.Gameover);
    }
}
