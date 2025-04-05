using UnityEngine;

public class AudioManagerVuforia : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public AudioClip boxingSound;
    public AudioClip gunSound;
    public AudioClip reloadSound;
    public AudioClip fencingSound;
    public AudioClip shieldSound;
    public AudioClip badmintonSound;
    public AudioClip golfSound;
    public AudioClip bombSound;


    public void PlayBoxingSound()
    {
        AudioSource.PlayClipAtPoint(boxingSound, Camera.main.transform.position);
    }
    public void PlayGunSound()
    {
        AudioSource.PlayClipAtPoint(gunSound, Camera.main.transform.position);
    }
    public void PlayReloadSound()
    {
        AudioSource.PlayClipAtPoint(reloadSound, Camera.main.transform.position);
    }

    public void PlayFencingSound()
    {
        AudioSource.PlayClipAtPoint(fencingSound, Camera.main.transform.position);
    }

    public void PlayShieldSound()
    {
        AudioSource.PlayClipAtPoint(shieldSound, Camera.main.transform.position);
    }
    public void PlayBadmintonSound()
    {
        AudioSource.PlayClipAtPoint(badmintonSound, Camera.main.transform.position);
    }
    public void PlayGolfSound()
    {
        AudioSource.PlayClipAtPoint(golfSound, Camera.main.transform.position);
    }
    public void PlayBombSound()
    {
        AudioSource.PlayClipAtPoint(bombSound, Camera.main.transform.position);
    }
}
