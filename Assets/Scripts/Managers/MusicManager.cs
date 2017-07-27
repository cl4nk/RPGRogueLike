using UnityEngine;

namespace Managers
{
    public class MusicManager : MonoBehaviour
    {
        private AudioSource source;

        // Use this for initialization
        private void Awake()
        {
            source = GetComponent<AudioSource>();
            GameManager.Instance.OnMainMenu += PlayMainMenuMusic;
            GameManager.Instance.OnLoading += PlayLoadingMusic;
            GameManager.Instance.OnPlaying += PlayGameMusic;
        }

        private void PlayMainMenuMusic()
        {
        }

        private void PlayLoadingMusic()
        {
        }

        private void PlayGameMusic()
        {
        }
    }
}