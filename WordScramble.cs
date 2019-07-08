using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;

[System.Serializable]
public class Result
{
    public int totalscore = 0;
    [Header("REF UI")]
    public Text textTime;
    public Text textTotalScore;
    public Text textHint;
    public Text textChances;
}
[System.Serializable]


public class Word
{
    public string word;
    [Header("leave empty if you want randomized")]
    public string desiredRandom;

    [Space(10)]
    public float timeLimit;

    public string GetString()
    {
        if (!string.IsNullOrEmpty(desiredRandom))
        {
            return desiredRandom;
        }

        string result = word;

        string scrambled_word = Scramble(word);

        while (scrambled_word == word)
        {
            scrambled_word = Scramble(word);
        }

        return scrambled_word;

    }

    private string Scramble(string word)
    {
        char[] array = word.ToCharArray();
        System.Random rng = new System.Random();
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
        return new string(array);
    }
}

public class WordScramble : MonoBehaviour
{
    public Word[] words;

    [Space(10)]
    public Result result;

    [Header("UI REFERENCE")]
    public CharObject prefab;
    public Transform container;
    public float space;
    public float lerpSpeed = 5;
    public int chance = 3;

    List<CharObject> charObjects = new List<CharObject>();
    CharObject firstSelected;

    public int currentWord;


    public static WordScramble main;

    private float totalScore;
    int length;
    int[] array;
    void Awake()
    {
        main = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        System.Random rand = new System.Random();
        chance = 3;
        length = words.Length;
        var rng = new System.Random();
        array = new int[length + 1];

        for (int i = 0; i <= length; i++)
        {
            array[i] = i;
        }

        for (int i = 0; i < length; i++)
        {
            int j = rand.Next(i, length);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        ShowScramble(array[currentWord]);
        result.textTotalScore.text = result.totalscore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        RepositionObject();

        totalScore = Mathf.Lerp(totalScore, result.totalscore, Time.deltaTime * 5);
        result.textTotalScore.text = Mathf.RoundToInt(totalScore).ToString();
    }

    void RepositionObject()
    {
        if (charObjects.Count == 0)
        {
            return;
        }

        float center = (charObjects.Count - 1) / 2;
        for (int i = 0; i < charObjects.Count; i++)
        {
            charObjects[i].rectTransform.anchoredPosition
                = Vector2.Lerp(charObjects[i].rectTransform.anchoredPosition,
                new Vector2((i - center) * space, 0), lerpSpeed * Time.deltaTime);
            charObjects[i].index = i;
        }
    }

    //public void ShowScramble()
    //{
    //    ShowScramble(UnityEngine.Random.Range(0, words.Length - 1));
    //}

    /// <summary>
    /// Show word from collection with desired index
    /// </summary>
    /// <param name="index">index of the element</param>
    public void ShowScramble(int index)
    {

        charObjects.Clear();
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        //WORDS FINISHED
        if (currentWord > words.Length - 1)
        {
            #if UNITY_EDITOR
            bool temp = EditorUtility.DisplayDialog("CONGRATULATIONS", "Your Score : " + Mathf.RoundToInt(totalScore) + "\nChance : " + chance, "Start Again", "Main Menu");
            if (temp == true)
            {
                SceneManager.LoadScene(2);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
            #endif
            return;
        }

        char[] chars = words[index].GetString().ToCharArray();
        foreach (char c in chars)
        {
            CharObject clone = Instantiate(prefab.gameObject).GetComponent<CharObject>();
            clone.transform.SetParent(container);

            charObjects.Add(clone.Init(c));
        }
        StartCoroutine(TimeLimit());
    }


    public void Swap(int indexA, int indexB)
    {
        CharObject tmpA = charObjects[indexA];

        charObjects[indexA] = charObjects[indexB];
        charObjects[indexB] = tmpA;

        charObjects[indexA].transform.SetAsLastSibling();
        charObjects[indexB].transform.SetAsLastSibling();

        CheckWord();
    }

    public void Select(CharObject charObject)
    {

        if (firstSelected)
        {
            Swap(firstSelected.index, charObject.index);

            //Unselect
            firstSelected.Select();
            charObject.Select();
        }
        else
        {
            firstSelected = charObject;
        }
    }
    public void UnSelect()
    {
        firstSelected = null;
    }
    public void CheckWord()
    {
        StartCoroutine(CoCheckWord());
    }
    
    IEnumerator CoCheckWord()
    {

        yield return new WaitForSeconds(0.5f);
        string word = "";
        foreach (CharObject charObject in charObjects)
        {
            word += charObject.character;
        }
        if (timeLimit <= 0)
        {
            if (word != words[array[currentWord]].word)
            {
                if (chance == 0)
                {
                    #if UNITY_EDITOR
                    bool temp = EditorUtility.DisplayDialog("GAME OVER", "Your Score : " + Mathf.RoundToInt(totalScore), "Start Again", "Main Menu");
                    if (temp == true)
                    {
                        SceneManager.LoadScene(2);
                    }
                    else
                    {
                        SceneManager.LoadScene(0);
                    }
                    #endif
                }
                else
                {
                    chance--;
                    #if UNITY_EDITOR    
                    EditorUtility.DisplayDialog("Oops", "You have " + chance + " chances left", "Continue");
                    #endif
                    currentWord++;
                    ShowScramble(array[currentWord]);
                    result.textChances.text = Mathf.RoundToInt(chance).ToString();
                    yield break;
                }
            }
            else
            {
                currentWord++;
                ShowScramble(array[currentWord]);
                yield break;
            }
        }
        if (word == words[array[currentWord]].word)
        {
            currentWord++;
            result.totalscore += Mathf.RoundToInt(timeLimit);

            ShowScramble(array[currentWord]);
        }
    }
    float timeLimit;

    IEnumerator TimeLimit()
    {
        timeLimit = ((words[currentWord].timeLimit) * level.percentage);
        result.textTime.text = Mathf.RoundToInt(timeLimit).ToString();
        int myWord = currentWord;
        hint.hints = words[array[currentWord]].word;

        yield return new WaitForSeconds(1);

        while (timeLimit > 0)
        {
            if (myWord != currentWord)
            {
                yield break;
            }
            timeLimit -= Time.deltaTime;
            result.textTime.text = Mathf.RoundToInt(timeLimit).ToString();
            yield return null;
        }
        CheckWord();
    }


}