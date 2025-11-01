using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundClick;
    [SerializeField] private AudioSource soundCorrectAnswer;

    public void PlaySoundClick()
    {
        if(soundClick!=null)
        soundClick.PlayOneShot(soundClick.clip);
    }

    public void PlayCorrectAnswer()
    {
        if (soundCorrectAnswer != null)
            soundCorrectAnswer.PlayOneShot(soundCorrectAnswer.clip);
    }
}
