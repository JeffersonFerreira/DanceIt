using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu]
public class Repository : ScriptableObject
{
    [SerializeField] private UserDanceModel[] _danceModels;

    public async Task<IReadOnlyCollection<UserDanceModel>> GetUserDances()
    {
        // Simulate a network request
        await Task.Delay(500);
        return await Task.FromResult(_danceModels);
    }
}