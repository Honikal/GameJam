using UnityEngine;

public class AudioManager : MonoBehaviour
{

    //Creamos la instancia de singleton
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        //Pasamos el sonidoque va a ser serializabele o modificable en el sistema
        public string name;                             //Nombre del sonido (para referencia sencilla)
        public AudioClip clip;                          //El clip o audio a reproducir
        [Range(0f, 1f)] public float volume = 1f;       //Volumen del sonido
        [Range(0.1f, 3f)] public float pitch = 1f;      //Pitch del sonido
        public bool loop = false;                       //Dejamos el loop como false para no reproducir

        [HideInInspector] public AudioSource source;    //El componente AudioSource para este sonido
    }

    public Sound[] sounds;                          //Lista de sonidos a reproducir

    //Evitamos que el singleton sea eliminado al recargar página
    private void Awake()
    {
        //Manejamos la función awake, para setear el singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //Ésto es para mantener el juego vivo entre escenas, y es opcional
        }
        else
        {
            //Si ya existe el objeto del singleton en una escena, no se ocupa agregar otro, lo destruimos
            Destroy(gameObject);
        }

        //Creamos componentes AudioSource por cada sonido
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    //Extrae un sonido basado en el nombre recibido
    public Sound GetSoundByName(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sonido: {name} no encontrado");
            return null;
        }
        return s;
    }

    //Reproduce un sonido basado en su nombre
    public void Play(string name)
    {
        Sound s = GetSoundByName(name);
        Debug.Log($"A punto de reproducir sonido: {name} con volumen {s.volume} y pitch {s.pitch}");
        s.source.Play();
    }

    //Detiene un sonido basado en su nombre
    public void Stop(string name)
    {
        Sound s = GetSoundByName(name);
        s.source.Stop();
    }

    //Asigna el pitch basado en el nombre
    public void SetPitch(string name, float pitch)
    {
        Sound s = GetSoundByName(name);
        s.source.pitch = pitch;
    }

    //Asigna el volumen basado en el nombre
    public void SetVolume(string name, float volume)
    {
        Sound s = GetSoundByName(name);
        s.source.volume = volume;
    }

}
