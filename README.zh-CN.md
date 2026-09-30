# Craft the Spire 开发与测试包

这份项目按你提供的 v0.1 策划实现 5 张正式牌、4 张临时矿物牌、4 件遗物和废弃矿井事件，并加入你随后要求的图鉴默认可见、全角色开局木镐和四张牌更容易获得的调整。目标只包含 STS2 public-beta 0.111.0 与 RitsuLib 0.6.2。已依据你的实际程序集核对 94 项接口签名。你已在 Windows 成功构建并安装上一版；本次新增改动仍需重新构建和游戏实测。开发版号保持 0.1.0。

已有完整 C# 源码、双语文本、原创生成美术、经过独立 Godot 挂载检查的 PCK、规则测试程序和官方上传工作区模板。这个模组没有设置页，也不会修改 SpireDraft；请在牌库、无色牌来源、遗物来源和事件中检查新内容。

## 本次调整

- 首次进入主菜单，木镐、铁镐、挖矿、TNT、金苹果自动加入当前档案的已发现记录；切换档案同样处理。四张临时矿物继续隐藏。
- 所有已注册角色新开局时，用木镐替换默认的一张打击。保留其他初始牌和牌组总张数，已有存档牌组不会被改写。自定义角色没有打击标签时，尝试替换一张基础攻击牌；完全没有这类攻击牌则保留原牌组。
- 铁镐、挖矿、TNT、金苹果在符合原版筛选的候选集合中，相对权重由 1 提高到 2。木镐保持 1。保留无色牌获取渠道、稀有度和去重规则，不加入普通有色战斗奖励池。权重归一化后，实际最终概率不会简单等于原概率的两倍。
- 矿物 40/35/20/5 的生成概率、卡牌效果与日系可爱封面保持原有设定。

## 已核对实际引用，现在编译

已依据你提供的 `CraftTheSpire-GameReferences-beta-20260930-233959.zip` 核对接口，结果见 `docs/game-api-check.json` 和 `docs/API-FIXES.md`。重新下载更新包，完整解压到一个新文件夹，双击 `Build.cmd`。不需要重复收集相同的游戏文件，也不需要重新安装 .NET 或 Godot。

构建前仍请确认游戏显示的版本是 0.111.0，并关闭游戏。脚本会检查当前 sts2.dll 的哈希与本次核对的文件一致；如果 Steam 更新了游戏或你选择了不同的安装目录，脚本会停止，并要求重新运行 `CollectGameReferences.cmd`。程序集版本号 0.1.0.0 本身不能证明游戏版本是 0.111.0。

你的已知游戏目录示例是 `D:\steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64`。移动整个项目文件夹不会改变脚本对项目内文件的定位。不要只移动单个 CMD 或 scripts 文件夹。

## 在 Windows 构建

你此前安装的 .NET 9 SDK 可以继续使用。双击 `Build.cmd`：先执行 C# 规则测试，再对当前游戏文件编译，最后校验并复制资源。整个过程会记录到项目根目录 `build-log.txt`。如果失败，请发送这个文件和第一条完整编译错误；窗口报错后不要使用旧 dist 文件来代表新构建。

本次只支持 Beta，所以脚本不再让你输入 stable/beta。它不会切换 Steam 分支，也不能仅凭目录名证明游戏版本。日志中的路径说明它究竟用了哪份游戏 DLL。

默认已经提供预先打好的 `assets/CraftTheSpire.pck`，构建时不需要安装 Godot。改变卡图或本地化之后，才需要用 Godot 4.5.1 运行 `PackAssets.cmd` 重新打包；资源哈希校验会阻止“改了 JSON 却忘记更新 PCK”。Godot 的普通编辑器即可处理这些纯资源，平台导出模板和 .NET 版编辑器都不是本包资源打包的必要条件。

## 安装与第一个测试

构建成功后，实际上传工作区是 `dist\CraftTheSpire-beta`，其中 `content` 必须恰好包含以下三个文件：

- `CraftTheSpire.dll`：C# 内容逻辑。
- `CraftTheSpire.pck`：图片与中英文文本。
- `CraftTheSpire.json`：游戏加载清单。

先关闭游戏，再双击 `InstallLocal.cmd`，或把上述三个文件复制到游戏的 `mods\CraftTheSpire\`。前置 RitsuLib 保持单独安装，不要把它的 DLL 或游戏 DLL 复制进本模组。使用 Steam 的模组启动方式进入游戏。

先确认模组列表有 Craft the Spire，再查看牌库中的木镐、铁镐、挖矿、TNT、金苹果。选择无色牌，清空搜索以及稀有度、类型、费用筛选后检查五张。木镐和挖矿为普通，铁镐和 TNT 为罕见，金苹果为稀有；仅显示前两张可能与稀有度筛选有关，需要结合日志确认。临时矿物继续隐藏。开一个新局检查木镐已在牌组中，再测试出牌生成矿物。完整过程见 `docs/TESTING.md`。

## 创意工坊

构建工作区符合 Mega Crit 官方上传器格式：根目录 `workshop.json`、`image.png`，可上传内容在 `content/`。下载官方上传器并解压，保持 Steam 登录；先用 `SetAuthor.cmd` 填写作者名，再重新构建。实际测试完成后运行 `Upload.cmd`，粘贴 `ModUploader.exe` 的完整路径。官方命令是 `ModUploader.exe upload -w <工作区目录>`。

首次默认私密。保留工作区里的 `mod_id.txt` 与脚本创建的 `workshop-identity.json`，以后更新会使用同一个条目；构建不会覆盖这些文件或已有的工坊说明。不要复制 SpireDraft 的工坊 ID 到本模组。上传脚本会拒绝没有本模组身份记录的已有 ID，或记录与 ID 不匹配的工作区。分支字段在 Steam 端可能有行为差异，还应在工坊页面核对支持版本。

公开首发按文档切到 1.0.0，并完成全部验收后再发布，详见 `docs/RELEASE.md`。工坊页面不能在测试前宣称联机和读档已通过。

## 实现中的技术细化

卡牌的目标枚举使用实际 API 的 `AnyEnemy` 与 `AllEnemies`。注册采用 RitsuLib 注解，并在初始化时注册程序集。后续要求的开局替换和权重调整通过 RitsuLib 管理的动态 Harmony 补丁接入；模型 ID 就绪后显式调用 ApplyDynamic，核对已应用数量，关键失败时回滚。原策划“不自行编写玩法补丁”的方案因此按你的新增要求调整。

开局补丁处理各角色 StartingDeck 的规范模型列表，之后仍由原版创建归属正确的玩家牌。权重补丁只处理 CardFactory 的六个最终抽选方法中的 Rng.NextItem<CardModel> 调用，不修改全局泛型 RNG、不重复注册卡牌。沿用调用方的原生随机流，每次权重抽选消耗一次 NextInt；原版颜色、稀有度、排除列表和其他条件继续决定候选牌。启动日志同时记录补丁的 registered/applied 数量，以及五张牌的牌池、可见和发现状态。上述行为仍需单人和联机实测。

战斗随机使用原版 `RunState.Rng.CombatCardGeneration`。事件使用原版 `EventModel.Rng`：它由本局种子与玩家 NetId 初始化，同一玩家在各客户端得到相同的流，避免同时选择事件时共用全局 RNG 的顺序差异。概率、奖励和稳定选项 ID 均不改。这里将文档“共享 Run RNG”的笼统描述细化为游戏原生事件流，须通过联机实测确认。

工程采用普通 Microsoft.NET.Sdk 编译逻辑，GodotSharp 4.5.1 与 net9.0 保持文档基线；纯资源由独立 Godot 4.5.1 打包。这避免让首次构建依赖平台导出模板。工作台、熔炉及开战加牌标记使用原生 SavedProperty，由 RitsuLib 接入模型保存与同步；真正的读档行为仍需要游戏验证。

岩浆使用原生 SetCurrentHp 命令实现向上取整、最低保留 1 HP 的精确生命损失。TNT 使用原生不可格挡、非加成的伤害命令，语义参照游戏中的自损牌。镐子击杀最后一个敌人后不再生成无意义的临时牌，与原版战斗结束行为一致。

## 文件索引

| 文件或目录 | 用途 |
| --- | --- |
| Code | 内容实现与纯规则 |
| CraftTheSpire/images | 游戏使用的卡图、遗物图、事件图 |
| CraftTheSpire/localization | zhs 与 eng 的 6 份文本表 |
| assets | 准备好的 PCK 和源文件哈希 |
| tests | 构建时执行的 C# 规则检查 |
| workshop | 官方格式模板，不能直接当成编译成品上传 |
| docs/VALIDATION.md | 哪些验证已完成，哪些仍待完成 |
| docs/TESTING.md | 游戏与联机验收步骤 |
| docs/RELEASE.md | 1.0.0 发布流程 |

官方上传器： https://github.com/megacrit/sts2-mod-uploader

RitsuLib： https://github.com/BAKAOLC/STS2-RitsuLib
