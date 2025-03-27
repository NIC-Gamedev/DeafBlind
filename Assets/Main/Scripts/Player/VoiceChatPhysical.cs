using System;
using System.Collections;
using FishNet.Object;
using FMOD;
using FMODUnity;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace Main.Scripts.Player
{
    public class VoiceChatPhysical : NetworkBehaviour
    {
        private Sound _sound;
        private CREATESOUNDEXINFO _exinfo;
        private Channel _channel;
        private ChannelGroup _channelGroup;

        public KeyCode PlayAndPause;
        public KeyCode ReverbOnSwith;

        private int _numOfDriversConnected = 0;
        private int _numOfDrivers = 0;
        
        [SerializeField] private int recordDeviceIndex = 0;
        [SerializeField] private string recordDeviceName;
        [SerializeField] private float latency;

        private Guid _micGuid;
        private int _sampleRate;
        private SPEAKERMODE _speakermode;
        private int _numOfChannels;
        private DRIVER_STATE _driverState;

        private bool _dspEnbled = false;
        private bool _playOrPause = true;
        private bool _playOkay = false;

        private byte[] _audioBuffer = new byte[1024]; // Буфер для передачи звука
        public override void OnStartClient()
        {
            if (IsOwner)
            {
                RuntimeManager.CoreSystem.getRecordNumDrivers(out _numOfDrivers, out _numOfDriversConnected);

                if (_numOfDriversConnected == 0)
                {
                    Debug.Log("Dont find any connected microphone");
                }
                else
                {
                    Debug.Log($"You have {_numOfDriversConnected} devices");
                }

                RuntimeManager.CoreSystem.getRecordDriverInfo(
                    recordDeviceIndex,
                    out recordDeviceName,
                    50,
                    out _micGuid,
                    out _sampleRate,
                    out _speakermode,
                    out _numOfChannels,
                    out _driverState
                );
                
                PhysicalAudioManager.instance.CreateSoundByTransform(ref _sound,_numOfChannels,_sampleRate,ref _exinfo, _channelGroup, _channel,transform,false,0f, 20f);
                RuntimeManager.CoreSystem.recordStart(recordDeviceIndex, _sound, true);

                StartCoroutine(Wait());
            }
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(latency);
            _channel.setPaused(_playOrPause);
            PhysicalAudioManager.instance.PlayPhysSound(ref _sound, ref _channelGroup, ref _channel, transform, true);
            _playOkay = true;
            Debug.Log("Ready To Play!");
        }

        private void Update()
        {
            if (_playOkay)
            {
                /*if (IsOwner)
                {
                    _channel.setVolume(0);                    
                }*/
                
                if (Input.GetKeyDown(PlayAndPause) )
                {
                    _playOrPause = !_playOrPause;
                }
                _channel.setPaused(_playOrPause);
                if (Input.GetKeyDown(ReverbOnSwith))
                {
                    REVERB_PROPERTIES propOn = PRESET.CONCERTHALL();
                    REVERB_PROPERTIES propOff = PRESET.OFF();
                    _dspEnbled = !_dspEnbled;
                    RuntimeManager.CoreSystem.setReverbProperties(1, ref _dspEnbled ? ref propOn : ref propOff);
                }
            }
        }
    }
}
