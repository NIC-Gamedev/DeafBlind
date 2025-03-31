using FishNet.Object;
using FMOD;
using FMODUnity;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class VoiceChatVirtual : NetworkBehaviour
{
    private Sound _recordSound;
    private Sound _playbackSound;
    private CREATESOUNDEXINFO _exinfo;
    private Channel _recordChannel;
    private Channel _playbackСhannel;
    private ChannelGroup _channelGroup;

    [SerializeField] private int recordDeviceIndex = 0;
    private int _sampleRate;
    private int _numOfChannels;
    
    private bool isRecording = false;
    private Coroutine _streamCoroutine;

    private MainController inputAction = InputManager.inputActions;

    public override void OnStartClient()
    {
        if (IsOwner)
        {
            // Получаем информацию о микрофоне
            RuntimeManager.CoreSystem.getRecordDriverInfo(
                recordDeviceIndex, out _, 50, out _, out _sampleRate, out _, out _numOfChannels, out _
            );

            // Создаём звук для записи
            AudioManager.instance.CreateSound(ref _recordSound, _numOfChannels, _sampleRate, ref _exinfo, _channelGroup, _recordChannel, false);
            RuntimeManager.CoreSystem.recordStart(recordDeviceIndex, _recordSound, true);

            // Включаем управление голосовым чатом
            inputAction.Player.VoiceChat.Enable();
            inputAction.Player.VoiceChat.performed += PlayVoice;
            inputAction.Player.VoiceChat.canceled += PlayVoice;
        }
    }

    private IEnumerator StreamAudio()
    {
        byte[] buffer;
        uint readPos = 0, writePos;
        IntPtr ptr1, ptr2;
        uint len1, len2;

        while (isRecording)
        {
            bool recording = false;
            RuntimeManager.CoreSystem.isRecording(recordDeviceIndex, out recording);
            if (!recording) yield break;

            RuntimeManager.CoreSystem.getRecordPosition(recordDeviceIndex, out writePos);

            uint length;
            if (writePos >= readPos)
                length = writePos - readPos;
            else
                length = _exinfo.length - readPos + writePos; // Циклический буфер
            if (length > 0)
            {
                RESULT result = _recordSound.@lock(readPos, length, out ptr1, out ptr2, out len1, out len2);
                if (result == RESULT.OK)
                {
                    buffer = new byte[len1 + len2];
                    Marshal.Copy(ptr1, buffer, 0, (int)len1);
                    if (ptr2 != IntPtr.Zero)
                    {
                        Marshal.Copy(ptr2, buffer, (int)len1, (int)len2);
                    }

                    _recordSound.unlock(ptr1, ptr2, len1, len2);

                    // Отправляем данные через FishNet
                    Debug.Log($"Sending {buffer.Length} bytes of voice data");
                    TransmitAudioServerRpc(buffer,_sampleRate,_numOfChannels);

                    readPos = (readPos + length) % _exinfo.length; // Обновляем с учетом кольцевого буфера
                }
            }

            yield return new WaitForSeconds(0.02f); // 20 мс задержка
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void TransmitAudioServerRpc(byte[] audioData,int sampleRate, int numOfchannels, NetworkConnection sender = null)
    {
        TransmitAudioObserversRpc(audioData,sampleRate,numOfchannels, sender.ClientId);
    }

    [ObserversRpc]
    private void TransmitAudioObserversRpc(byte[] audioData,int sampleRate, int numOfchannels, int senderClientId)
    {
        if (senderClientId == NetworkManager.ClientManager.Connection.ClientId)
            return;

        OnReceiveVoice(audioData,sampleRate,numOfchannels);
    }



    private void OnReceiveVoice(byte[] voicePacket,int sampleRate, int numOfchannels)
    {
        byte[] audioData = voicePacket;
        int lengthToShow = Math.Min(50, audioData.Length);
        if (!_playbackSound.hasHandle())
        {
            Debug.Log($"Sample Rate: {sampleRate}. Num of Channels: {numOfchannels}");
            AudioManager.instance.CreateSound(audioData, ref _playbackSound, numOfchannels, sampleRate, ref _exinfo, _channelGroup, _playbackСhannel, false);
            AudioManager.instance.PlaySound(ref _playbackSound, ref _channelGroup, ref _playbackСhannel);
            _playbackСhannel.setPaused(false);
        }
    }

    public void PlayVoice(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton()) // Начать запись
        {
            if (_streamCoroutine == null)
            {
                isRecording = true;
                _streamCoroutine = StartCoroutine(StreamAudio());
            }
        }
        else // Остановить запись
        {
            if (_streamCoroutine != null)
            {
                isRecording = false;
                StopCoroutine(_streamCoroutine);
                _streamCoroutine = null;
            }
        }
        _playbackСhannel.setPaused(!isRecording);
    }

    private void OnDestroy()
    {
        inputAction.Player.VoiceChat.performed -= PlayVoice;
        inputAction.Player.VoiceChat.canceled -= PlayVoice;
        
        if (_recordSound.hasHandle()) _recordSound.release();
        if (_playbackSound.hasHandle()) _playbackSound.release();
    }
}
