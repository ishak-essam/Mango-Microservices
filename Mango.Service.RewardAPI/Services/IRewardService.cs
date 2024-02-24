

using Mango.Service.RewardAPI.Message;

namespace Mango.Service.RewardAPI.Services
{
    public interface IRewardService
    {
        Task UpdateReward ( RewardMessage  rewardMessage );
    }
}
