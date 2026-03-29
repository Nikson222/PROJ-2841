using _Scripts._Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts._Infrastructure
{
    public class MaxScoreDisplayer : MonoBehaviour
    {
        [SerializeField] private Text _text;

        private PlayerProfile _playerProfile;

        [Inject]
        private void Construct(PlayerProfile playerProfile)
        {
            _playerProfile = playerProfile;
        }

        private void Start()
        {
            UpdateView();
        }

        private void OnEnable()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            if (_text == null || _playerProfile == null)
                return;

            _text.text = _playerProfile.MaxScore.ToString();
        }
    }
}
