using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ChipNDale
{
    public class StaticMapView : MonoBehaviour
    {

        public static StaticMapView Instance=null;

        public RectTransform lifesParent;
        public GameObject lifePrefab;


        public CanvasGroup GameOver;
        public CanvasGroup Win;
        public CanvasGroup NewGame;
        
        public AudioClip menu;
        public AudioClip boss;
        public AudioClip gameover;
        public AudioClip win;

        public AudioClip damageBoss;
        public AudioClip damageChip;
        public AudioClip jump;

        public AudioSource Sfx;
        public AudioSource AudioSource;


        private List<GameObject> lifes=new List<GameObject>();
        private bool Started = false;
        private Coroutine fade;
        private Canvas canvas;


        public void PlayJump()
        {
            /*AudioSource.clip = jump;
            AudioSource.loop = false;
            AudioSource.Play();*/
        }

        public void PlayDamageBoss()
        {
            /*AudioSource.clip = damageBoss;
            AudioSource.loop = false;
            AudioSource.Play();*/
        }

        public void PlayChipDamage()
        {
            /*AudioSource.clip = damageChip;
            AudioSource.loop = false;
            AudioSource.Play();*/
        }
        
        
        
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            Started = false;
            Time.timeScale = 0;
            AudioSource.clip = menu;
            AudioSource.loop = true;
            AudioSource.Play();
            canvas=lifesParent.GetComponentInParent<Canvas>();
        }


        public void SetHP(int hp)
        {
            if (hp >= 0)
            {
                if (lifes.Count > hp)
                {
                    Destroy(lifes[lifes.Count-1]);
                    lifes.RemoveAt(lifes.Count - 1);
                }
                else
                {
                    while (hp > lifes.Count)
                    {
                        var live = Instantiate(lifePrefab, lifesParent);
                        lifes.Add(live);
                    }
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(lifesParent);
            }
        }
        
        
        public void ShowGameOver()
        {
            AudioSource.clip = gameover;
            AudioSource.loop = true;
            AudioSource.Play();
            Time.timeScale = 0;
            if (fade != null) StopCoroutine(fade);
            fade = StartCoroutine(ShowCoroutine(GameOver));
        }
        
        public void HideGameOver()
        {
            Time.timeScale = 1;
            if (fade != null) StopCoroutine(fade);
            fade = StartCoroutine(HideCoroutine(GameOver));
        }
        
        public void ShowNewGame()
        {
            Time.timeScale = 0;
            if (fade != null) StopCoroutine(fade);
            fade = StartCoroutine(ShowCoroutine(NewGame));
        }
        
        public void HideNewGame()
        {
            AudioSource.clip = boss;
            AudioSource.loop = true;
            AudioSource.Play();
            Time.timeScale = 1;
            if (fade != null) StopCoroutine(fade);
            fade = StartCoroutine(HideCoroutine(NewGame));
        }

        private IEnumerator ShowCoroutine(CanvasGroup canvasGroup)
        {
            canvasGroup.gameObject.SetActive(true);
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.unscaledDeltaTime;
                yield return null;
            }
            canvasGroup.alpha = 1;
        }
        
      

        private IEnumerator HideCoroutine(CanvasGroup canvasGroup)
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -=  Time.unscaledDeltaTime;
                yield return null;
            }
            canvasGroup.alpha = 0;
            canvasGroup.gameObject.SetActive(false);
        }


        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void ShowWin()
        {
            AudioSource.clip = win;
            AudioSource.loop = true;
            AudioSource.Play();
            Time.timeScale = 0;
            if (fade != null) StopCoroutine(fade);
            fade = StartCoroutine(ShowCoroutine(Win));
        }
    }
}
