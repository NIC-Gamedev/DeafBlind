using FishNet.Object;
using FMOD;
using FMODUnity;
using System;
using System.Collections;
using FishNet.Connection;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class VoiceChatVirtual : NetworkBehaviour
{
    private Sound _recordSound;
    private Sound _playbackSound;
    private CREATESOUNDEXINFO _recordExinfo;
    private CREATESOUNDEXINFO _playbackExinfo;
    private Channel _recordChannel;
    private Channel _playbackСhannel;
    private ChannelGroup _channelGroup;

    [SerializeField] private int recordDeviceIndex = 0;
    [SerializeField] private int _sampleRate;
    [SerializeField] private int _numOfChannels;
    [SerializeField] private string NAME;
    
    private bool isRecording = false;
    private Coroutine _streamCoroutine;

    private MainController inputAction = InputManager.inputActions;
    [SerializeField] private uint currWritePos;

    public int bufferMultiplier = 1;
    
    bool isSoundValid = false;
    bool isChannelValid = false;
    public override void OnStartClient()
    {
        if (IsOwner)
        {
            // Получаем информацию о микрофоне
            RuntimeManager.CoreSystem.getRecordDriverInfo(
                recordDeviceIndex, out NAME, 50, out _, out _sampleRate, out _, out _numOfChannels, out _
            );
            // Создаём звук для записи
            AudioManager.instance.CreateSound(out _recordSound, _numOfChannels, _sampleRate, ref _recordExinfo, _channelGroup, _recordChannel, false);
            RuntimeManager.CoreSystem.recordStart(recordDeviceIndex, _recordSound, true);

            // Включаем управление голосовым чатом
            inputAction.Player.VoiceChat.Enable();
            inputAction.Player.VoiceChat.performed += PlayVoice;
            inputAction.Player.VoiceChat.canceled += PlayVoice;
            //StartCoroutine(Wait());
            FMOD.DSP highpass;
            RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.HIGHPASS, out highpass);
            highpass.setParameterFloat((int)FMOD.DSP_HIGHPASS.CUTOFF, 100.0f); // Гц
            _playbackСhannel.addDSP(0, highpass);
            FMOD.DSP normalize;
            RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.NORMALIZE, out normalize);
            normalize.setParameterFloat((int)FMOD.DSP_NORMALIZE.FADETIME, 1.0f);
            normalize.setParameterFloat((int)FMOD.DSP_NORMALIZE.MAXAMP, 2.0f); // максимальное усиление
            _playbackСhannel.addDSP(1, normalize);   
        }

    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        AudioManager.instance.PlaySound( _recordSound,  _channelGroup, out _recordChannel);
        _recordChannel.setPaused(false);
    }

    private IEnumerator StreamAudio()
    {
        uint bufferBytes = (uint)(_sampleRate  * sizeof(short) * 0.1f);
        IntPtr ptr1, ptr2;
        uint len1, len2;
        uint lastRecordPos = 0;
        while (isRecording)
        {
            RuntimeManager.CoreSystem.getRecordPosition(recordDeviceIndex, out uint currentWritePos);
            if (currentWritePos == lastRecordPos)
            {
                yield return null;
                continue;
            }
            lastRecordPos = currentWritePos;
            RESULT lockResult = _recordSound.@lock(currentWritePos * sizeof(short), (uint)bufferBytes, out ptr1, out ptr2, out len1, out len2);
            currWritePos = currentWritePos;
            if (lockResult == RESULT.OK)
            {
                short[] audioData = GetDataFromLock(ptr1, ptr2, len1, len2);
                _recordSound.unlock(ptr1, ptr2, len1, len2);
            
            
                if (isSoundValid)
                {
                    _playbackSound.release();
                    isSoundValid = false;
                }
                
                TransmitAudioServerRpc(audioData, _sampleRate, _numOfChannels);
            }

            yield return new WaitForSeconds(0.01f);
        }
    
        if (isSoundValid) _playbackSound.release();
    }

    
    public unsafe short[] GetDataFromLock(IntPtr ptr1, IntPtr ptr2, uint len1, uint len2)
    {
        // Общий размер в семплах
        short[] samples = new short[(len1 + len2) / sizeof(short)];
    
        // Копируем данные через указатели
        byte* src1 = (byte*)ptr1.ToPointer();
        byte* src2 = (byte*)ptr2.ToPointer();

        fixed (short* target = samples)
        {
            byte* dst = (byte*)target;
        
            // Копируем первую часть
            Buffer.MemoryCopy(src1, dst, len1, len1);
        
            // Копируем вторую часть (если есть)
            if (len2 > 0)
            {
                Buffer.MemoryCopy(src2, dst + len1, len2, len2);
            }
        }

        return samples;
    }

    [ServerRpc(RequireOwnership = false)]
    private void TransmitAudioServerRpc(short[] audioData,int sampleRate, int numOfchannels, NetworkConnection sender = null)
    {
        TransmitAudioObserversRpc(audioData,sampleRate,numOfchannels, sender.ClientId);
    }

    [ObserversRpc]
    private void TransmitAudioObserversRpc(short[] audioData,int sampleRate, int numOfchannels, int senderClientId)
    {
        if (senderClientId == NetworkManager.ClientManager.Connection.ClientId)
            return;

        OnReceiveVoice(audioData,sampleRate,numOfchannels);
    }



    private void OnReceiveVoice(short[] voicePacket,int sampleRate, int numOfchannels)
    {
        short[] audioData = voicePacket;
        _playbackСhannel.getNumDSPs(out var numdsps);
        Debug.Log($"DSPS:{numdsps}");
        var result = AudioManager.instance.CreateSound(
            audioData, 
            out _playbackSound, 
            numOfchannels, 
            sampleRate, 
            ref _playbackExinfo, 
            _channelGroup, 
            _playbackСhannel, 
            false
        );

        if (result == RESULT.OK)
        {
            isSoundValid = true;
            result = AudioManager.instance.PlaySound(_playbackSound, _channelGroup, out _playbackСhannel);
            isChannelValid = (result == RESULT.OK);

            // Добавляем DSP после создания канала
            /*if (isChannelValid)
            {
                // Highpass DSP
                FMOD.DSP highpass;
                RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.HIGHPASS, out highpass);
                highpass.setParameterFloat((int)FMOD.DSP_HIGHPASS.CUTOFF, 100.0f);
                _playbackСhannel.addDSP(0, highpass);

                // Normalize DSP
                FMOD.DSP normalize;
                RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.NORMALIZE, out normalize);
                normalize.setParameterFloat((int)FMOD.DSP_NORMALIZE.FADETIME, 1.0f);
                normalize.setParameterFloat((int)FMOD.DSP_NORMALIZE.MAXAMP, 2.0f);
                _playbackСhannel.addDSP(3, normalize);
                
                FMOD.DSP compressor;
                RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.COMPRESSOR, out compressor);
                compressor.setParameterFloat((int)FMOD.DSP_COMPRESSOR.THRESHOLD, -20.0f);
                compressor.setParameterFloat((int)FMOD.DSP_COMPRESSOR.RATIO, 4.0f);
                _playbackСhannel.addDSP(1, compressor);
                
                FMOD.DSP aec;
                RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.ECHO, out aec);
                aec.setParameterFloat((int)FMOD.DSP_ECHO.DELAY, 0.1f); // Уменьшение времени эха
                _playbackСhannel.addDSP(2, aec);
            }*/
        }
        _playbackСhannel.setPaused(false);
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
