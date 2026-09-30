# 实际游戏程序集接口修正

依据用户提供的 `CraftTheSpire-GameReferences-beta-20260930-233959.zip`，核对游戏及 RitsuLib 0.6.2 的实际程序集元数据，修正以下调用。

| 使用位置 | 实际接口要求与修正 |
| --- | --- |
| 木镐、铁镐、TNT 的攻击 | `FromCard(this, cardPlay)`，必须传入本次出牌记录 |
| TNT 自身生命损失 | `CreatureCmd.Damage` 的卡牌来源重载必须同时传入 `this` 和 `cardPlay` |
| 金苹果获得力量 | `PowerCmd.Apply` 的首个参数必须是 `choiceContext` |
| 末影箱、钻石镐开局钩子 | `BeforeHandDraw` 的战斗参数为 `ICombatState` |
| 生成矿物的工具方法 | 接受 `ICombatState`，与 CardModel.CombatState 一致 |
| 事件每局限一次 | `VisitedEventIds` 属于具体 `RunState`，通过类型检查访问；不在 `IRunState` 上直接调用 |
| 再次构建 | 检查游戏 DLL 哈希与本次核对文件一致，防止更新游戏后误用这份源码 |
| 五张正式牌默认可见 | 等待模型、主菜单与当前档案就绪，使用 ProgressState.MarkCardAsSeen 并在新增发现时保存一次 |
| 档案切换 | ProfileDataReadyEvent 后延迟处理发现记录，避免在原版档案初始化调用栈中写入 |
| 每个角色开局一张木镐 | 找到各角色实际 StartingDeck getter，后置替换一张 Strike 标签牌；无标签时采用 Basic/Attack 回退 |
| 四张牌两倍相对权重 | 实际 CardFactory 六个重载各含一次实例 Rng.NextItem<CardModel>；替换为保留同一 RNG 和候选牌的静态加权抽选 |
| 动态补丁真正应用 | RitsuLib CreatePatcher + DynamicPatchBuilder + ApplyDynamic，关键失败回滚，并检查 AppliedPatchCount 与目标数一致 |
| 上版可空值警告 | 显式检查战斗状态和事件拥有者，再传入原生命令；此次尚未通过编译器复核 |

94 项方法签名、相关枚举成员及 52 处可重写成员已核对；25 个 C# 文件和 8 个 PowerShell 文件通过语法解析。六个抽选方法的原始 IL 已核对，摘要见 `card-selection-il.json`。这些检查不能代替编译与实际补丁应用。

用户截图证明上一次接口修正版在 Windows 构建通过，执行了 40,205 项规则检查，并成功安装。本次加入图鉴和获取调整后的源码尚未编译；当前环境无法启动 .NET CoreCLR，下一步仍须在 Windows 上运行 Build.cmd。纯规则测试新增了完整加权抽选分布、非法输入和不同初始牌组的替换检查。

此包的游戏内 ID 为 CraftTheSpire。工坊封面采用用户要求的日系可爱风。实际游戏 DLL 仅用于核对，不包含在交付包或工坊内容中。
