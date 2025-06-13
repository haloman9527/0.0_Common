
namespace Atom
{
    public class AudioManager : SingletonBase<AudioManager>, IAudioManager
    {
        private IAudioManager o;

        public void Install(IAudioManager o)
        {
            this.o = o;
        }

        public void Init(bool force = false)
        {
            this.o.Init(force);
        }

        public void Play(int audioId)
        {
            o.Play(audioId);
        }
    }
}