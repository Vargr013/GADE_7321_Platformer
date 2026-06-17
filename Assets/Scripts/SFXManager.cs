using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    private SoundHashMap soundMap;

    private AudioSource audioSource;

    [Header("Sound Clips")]
    public AudioClip[] footstepClips;
    public AudioClip enemySpawn;
    public AudioClip playerDeath;
    public AudioClip platformMove;
    public AudioClip checkpointReached;
    public AudioClip droneFly;

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

        soundMap.Add("EnemySpawn", enemySpawn);
        soundMap.Add("PlayerDeath", playerDeath);
        soundMap.Add("PlatformMove", platformMove);
        soundMap.Add("CheckpointReached", checkpointReached);
        soundMap.Add("DroneFly", droneFly);
    }

    public void PlaySound(string soundName)
    {
        AudioClip clip = null;

        if (soundName == "Footstep" && footstepClips != null && footstepClips.Length > 0)
        {
            clip = footstepClips[Random.Range(0, footstepClips.Length)];
        }
        else
        {
            clip = soundMap.Get(soundName);
        }

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Sound not found: " + soundName);
        }
    }
}