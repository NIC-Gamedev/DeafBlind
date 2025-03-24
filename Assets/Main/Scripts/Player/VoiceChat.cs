using System.Collections;
using FishNet.Object;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Main.Scripts.Player
{
    public class VoiceChat : NetworkBehaviour
    {
          private const int SampleRate = 48000;
          private const int ClipBufferSize = 1024;
          private AudioClip _microphoneClip;
          private bool _isTransmitting;

          [SerializeField] private EventReference voiceEvent;
          private EventInstance _voiceInstance;

          public override void OnStartClient()
          {
              base.OnStartClient();
              if (IsOwner)
              {
                  if (Microphone.devices.Length > 0)
                  {
                      _microphoneClip = Microphone.Start(Microphone.devices[0], true, 1, SampleRate);
                      StartCoroutine(StreamVoiceCoroutine());   
                  }
              }
          }

          private IEnumerator StreamVoiceCoroutine()
          {
              float[] audioBuffer = new float[ClipBufferSize];
              int prevPos = 0;
        
              while (IsOwner)
              {
                  int currPos = Microphone.GetPosition(null);
                  if (currPos < prevPos) currPos = SampleRate;
            
                  if (currPos - prevPos >= ClipBufferSize)
                  {
                      _microphoneClip.GetData(audioBuffer, prevPos);
                      TransmitVoiceRpc(audioBuffer);
                      prevPos = (prevPos + ClipBufferSize) % SampleRate;
                  }
                  yield return null;
              }
          }

          [ServerRpc]
          private void TransmitVoiceRpc(float[] audioData)
          {
              BroadcastVoiceObserversRpc(audioData);
          }

          [ObserversRpc]
          private void BroadcastVoiceObserversRpc(float[] audioData)
          {
              if (!IsOwner) PlayVoice(audioData);
          }

          private void PlayVoice(float[] audioData)
          {
              if (!_voiceInstance.isValid())
              {
                  _voiceInstance = PhysicalAudioManager.instance.PlayByTransform(
                      voiceEvent, 
                      transform,
                      minDistance: 5f,
                      maxDistance: 20f
                  );
              }
        
              _voiceInstance.setParameterByName("VoiceData", ConvertAudioToIntensity(audioData));
          }

          private float ConvertAudioToIntensity(float[] data)
          {
              float sum = 0;
              foreach (var sample in data) sum += Mathf.Abs(sample);
              return Mathf.Clamp01(sum / data.Length * 10f);
          }

          public override void OnStopClient()
          {
              base.OnStopClient();
              _voiceInstance.stop(STOP_MODE.IMMEDIATE);
              _voiceInstance.release();
          }
    }
}
