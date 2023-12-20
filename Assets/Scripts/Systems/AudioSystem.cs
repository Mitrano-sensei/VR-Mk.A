using System;
using UnityEngine;

/// <summary>
/// Insanely basic audio system which supports 3D sound.
/// Ensure you change the 'Sounds' audio source to use 3D spatial blend if you intend to use 3D sounds.
/// </summary>
public class AudioSystem : StaticInstance<AudioSystem> {
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _soundsSource;
    [SerializeField] private AudioClip _collisionClip;

    public void PlayMusic(AudioClip clip) {
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void PlaySound(AudioClip clip, Vector3 pos, float vol = 1) {
        _soundsSource.transform.position = pos;
        PlaySound(clip, vol);
    }

    public void PlaySound(AudioClip clip, float vol = 1) {
        _soundsSource.PlayOneShot(clip, vol);
    }

    public void PlayCollisionSound(CollisionEvent collisionEvent)
    {
        if (collisionEvent.Collision.relativeVelocity.magnitude > 1)
        {
            PlaySound(_collisionClip,
                collisionEvent.Collision.transform.position,
                Math.Min(
                    (float) Math.Sqrt(collisionEvent.Collision.relativeVelocity.magnitude / 4),
                    10)
            );
        }
    }
}