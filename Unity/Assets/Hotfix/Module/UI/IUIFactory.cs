using ETModel;
using UnityEngine;

namespace ETHotfix
{
	public interface IUIFactory
	{
		UI Create();
        void Start();
        ETTask OnEnable();//这里的ETTask,是为了处理必须等待一段动画结束,界面才出现的情况.如果一开始的动效就需要UI显示,请自行设置UI的GameObject显示
        ETTask OnDisable();//这里的ETTaskCompletionSource,是为了等待消失动效结束,界面才消失的情况
    }
}