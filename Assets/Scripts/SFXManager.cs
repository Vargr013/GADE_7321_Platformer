using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    private SoundHashMap soundMap;

    private AudioSource audioSource;

    [Header("Sound Clips")]
    public AudioClip footstep;
    public AudioClip enemySpawn;
    public AudioClip playerDeath;
    public AudioClip platformMove;
    public AudioClip checkpointReached;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();

        soundMap = new SoundHashMap();

        soundMap.Add("Footstep", footstep);
        soundMap.Add("EnemySpawn", enemySpawn);
        soundMap.Add("PlayerDeath", playerDeath);
        soundMap.Add("PlatformMove", platformMove);
        soundMap.Add("CheckpointReached", checkpointReached);
    }

    public void PlaySound(string soundName)
    {
        AudioClip clip = soundMap.Get(soundName);

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Sound not found in HashMap: " + soundName);
        }
    }
}