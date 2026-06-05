using DeliveryRushExam.Data;
using System.Threading.Tasks;
using UnityEngine;

// Interface ISaveService
public interface ISaveService
{
    Task<PlayerProgressData> LoadAsync();
    Task SaveAsync(PlayerProgressData progressData);
}