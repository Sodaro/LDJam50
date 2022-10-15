using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private int _sceneIndexToLoad = 1;
    public void StartLevelChange()
    {
        StartCoroutine(FadeOut());
    }
    private IEnumerator FadeOut()
    {
        Color a = Color.black;
        Color b = a;

        a.a = 0;
        float f = 0;
        float time = 3f;
        while (f < time)
        {
            f += Time.deltaTime;
            _fadeImage.color = Color.Lerp(a, b, f / time);
            yield return null;
        }
        SceneManager.LoadScene(_sceneIndexToLoad);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement player))
        {
            StartLevelChange();
        }
    }
}
