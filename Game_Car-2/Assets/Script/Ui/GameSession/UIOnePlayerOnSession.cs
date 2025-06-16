using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UIOnePlayerOnSession : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _id;
    [SerializeField] private TextMeshProUGUI _selectedCar;
    [SerializeField] private Toggle _isReady;

    private NetworkPlayerProfile _profile;
    private bool _isSetup = false;

    public void Set(NetworkPlayerProfile profile)
    {
        _profile = profile;

        // Логика интерактивности — только локальный игрок может менять статус "готов"
        if (_profile.isLocalPlayer)
        {
            _isReady.interactable = true;
            _isReady.onValueChanged.AddListener(OnReadyToggled);
        }
        else
        {
            _isReady.interactable = false;
        }

        _isSetup = true;
    }

    private void Update()
    {
        if (!_isSetup || _profile == null)
            return;

        
        _name.text = _profile.playerName;
        _id.text = $"ID: {_profile.playerID}";
        _selectedCar.text = $"Машина #{_profile.selectedCarIndex}";
        _isReady.isOn = _profile.isReady;
    }

    private void OnReadyToggled(bool isReady)
    {
        if (_profile != null && _profile.isLocalPlayer)
        {
            _profile.CmdSetReady(isReady); // Mirror Command
        }
    }
}
