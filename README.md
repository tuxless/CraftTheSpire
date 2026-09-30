# CraftTheSpire
杀戮尖塔2 CraftTheSpire小型我的世界内容包
我的世界灵感 × 杀戮尖塔 2 · 方块生存小型内容包
 
带好木镐。下一块石头下面，也许就是钻石。

挥一镐，敲开敌人的防线；挖一块煤，多打一张关键牌；攒几份材料，让工作台替你续上手牌。遇到一群难缠的家伙，还可以掏出 TNT——点火之前，记得看一眼自己的血条。
Craft the Spire 是一款受《我的世界》启发的《杀戮尖塔 2》非官方内容模组。它围绕挖矿、临时资源和遗物搭配，为原有角色加入一场小小的方块冒险。
已在 Steam 创意工坊发布。 在《杀戮尖塔 2》创意工坊搜索 “我的世界小型内容包 Craft the Spire [Public Beta]”，订阅本模组与 RitsuLib 后即可开始冒险。这个仓库提供模组源码、图片资源、双语文本与构建脚本。
项目	目标与支持范围
游戏	Slay the Spire 2 public-beta 0.111.0
发布渠道	Steam 创意工坊
必需前置	RitsuLib 0.6.2
语言	简体中文、英文
内容	5 张正式卡牌、4 张临时矿物牌、4 件遗物、1 个事件
源码构建	Windows、.NET 9 SDK、C# 13


以上是本项目的固定适配目标，不代表游戏或依赖的最新版本。其他游戏分支、更新版本及与其他模组的组合，需另行验证。模组版本以仓库中的 CraftTheSpire.json 为准。
从第一场战斗开始挖矿
- 开局木镐：每个原版角色新开局时，一张初始“打击”替换为木镐，牌组总张数不变。
- 图鉴默认可见：进入主菜单后，五张正式卡牌自动标记为已发现，方便查看和规划搭配。
- 更容易遇到新卡：铁镐、挖矿、TNT、金苹果在符合条件的原有无色牌候选中，相对抽取权重提高到 2；木镐保持 1。原有获取渠道与稀有度继续生效，最终概率会按候选集合归一化。
- 资源当场使用：四种矿物都是 0 费临时技能，带有消耗、虚无；它们帮助本场战斗，不加入永久牌组。
- 遗物形成搭配：熔炉让煤炭烧得更旺，工作台把矿物转成抽牌，末影箱与钻石镐则准备好你的开工资源。
- 矿井里的抉择：平稳拿走木镐，付出代价换遗物，或者赌一次钻石与岩浆。
开局替换仅作用于新局，已有存档不会额外发放木镐。自定义角色若具有打击标签，同样尝试替换一张打击；没有该标签时回退到一张基础攻击牌，没有适合的牌则保留原牌组。其他角色模组的兼容性需实际测试。
卡牌与矿物
五张正式无色卡牌
以下为基础数值，实际结算仍会受到原版相关增益等效果影响。
卡牌	稀有度	费用	基础效果	升级效果
木镐 Wooden Pickaxe	普通	1	造成 6 点伤害，生成 1 张随机矿物加入手牌	伤害提升至 9
铁镐 Iron Pickaxe	罕见	1	造成 7 点伤害，生成 1 张铁锭加入手牌	伤害提升至 10
挖矿 Mining	普通	2	生成 2 张随机矿物加入手牌	费用降至 1
TNT	罕见	2	对全体敌人造成 18 点伤害，自损 4 点生命，消耗	伤害提升至 24
金苹果 Golden Apple	稀有	2	获得 12 点格挡与 1 点力量，消耗	格挡提升至 16


TNT 的 4 点自损不能被格挡抵消。金苹果的升级不增加力量。
四种临时矿物
矿物	随机挖矿概率	效果
煤炭 Coal	40%	获得 1 点能量
铁锭 Iron Ingot	35%	获得 6 点格挡
金锭 Gold Ingot	20%	抽 1 张牌，获得 3 点格挡
钻石 Diamond	5%	获得 1 点能量，抽 1 张牌


矿物均为 0 费、消耗、虚无、不可升级，并隐藏于卡牌图鉴。打出后消耗，回合结束时仍留在手牌中的矿物也会因虚无被消耗。金锭提供抽牌与格挡，不会增加金币；铁镐固定生成铁锭，不参与随机矿物抽取。
四件共享遗物
遗物	稀有度	效果
熔炉 Furnace	普通	每场战斗第一次打出煤炭时，额外获得 1 点能量
工作台 Crafting Table	罕见	本场战斗中，你每打出 3 张矿物牌，抽 1 张牌；新战斗重新计数
末影箱 Ender Chest	罕见	每场战斗第一次抽牌前，生成 1 张随机矿物加入手牌
钻石镐 Diamond Pickaxe	稀有	每场战斗第一次抽牌前，生成 1 张钻石加入手牌


这些遗物加入共享遗物池，各原版角色都有机会获得。工作台只计算其拥有者打出的矿物牌。
废弃矿井
每局至多出现一次的矿井探险。木镐就在眼前，深处却可能藏着更好的东西。
<details>
<summary>查看四个选项与具体结果（含剧透）</summary>

选择	结果
拿走木镐	将一张木镐加入永久牌组，可以获得重复的木镐
深入矿井	失去 5 点最大生命，随机获得尚未拥有的熔炉、工作台或末影箱；三件都已拥有时，改为获得 75 金币且不扣最大生命
向下挖掘	50% 获得钻石镐，50% 遇到岩浆；已有钻石镐时，此选项锁定
离开	平安离开，没有额外收益或损失


岩浆造成的当前生命损失为最大生命的 15%，向上取整，但至少保留 1 点生命。深入矿井的最大生命代价不具有这一非致死保证。
</details>

安装与游玩
Steam 创意工坊订阅（推荐）
1. 在 Steam 中将游戏切换到本模组适配的 public-beta 分支，并确认实际游戏版本匹配上表。
2. 打开《杀戮尖塔 2》的创意工坊，搜索 “我的世界小型内容包 Craft the Spire [Public Beta]” 并订阅。
3. 同时订阅并启用 RitsuLib，等待 Steam 下载完成。
4. 通过 Steam 的模组启动方式进入游戏，确认模组列表中已启用 Craft the Spire 与 RitsuLib。
5. 新开一局，带着初始木镐开始挖矿；五张正式牌可在无色牌图鉴中查看。
工坊版本可随订阅接收更新。若之前手动安装过本模组，请移走本地 mods/CraftTheSpire/ 副本，保留工坊这一种加载来源。
使用已编译的模组包
1. 准备匹配的游戏版本，单独安装并启用 RitsuLib 0.6.2。
2. 关闭游戏，将本模组的三个发布文件放入游戏目录下的 mods/CraftTheSpire/：
   文件	用途
   CraftTheSpire.dll	卡牌、遗物、事件与接入逻辑
   CraftTheSpire.pck	图片与中英文文本资源
   CraftTheSpire.json	模组加载清单
3. 通过 Steam 的模组启动方式进入游戏，确认模组列表中显示 Craft the Spire。
4. 在图鉴选择无色牌并清空筛选，检查五张正式卡牌；新开一局，确认牌组中的木镐。
本地安装与工坊订阅应只保留一种加载来源，避免同时加载两个相同 ID 的副本。RitsuLib 保持独立安装。
从源码构建
需要 .NET 9 SDK 和目标游戏安装目录中的实际程序集。克隆或下载源码后，在包含 CraftTheSpire.csproj 的项目根目录执行：
.\SetAuthor.cmd
.\Build.cmd
SetAuthor.cmd 用于设置加载清单中的作者名。Build.cmd 会定位或询问游戏数据目录，核对游戏 DLL 与资源哈希，运行纯规则测试，然后编译并生成 dist/CraftTheSpire-beta/。看到 BUILD PASSED 后，可执行：
.\InstallLocal.cmd
脚本会安装刚构建的三个发布文件。构建日志保存在项目根目录的 build-log.txt。
工程使用 net9.0、GodotSharp 4.5.1 与固定版本的 STS2.RitsuLib 0.6.2。游戏程序集用于本机编译，不随仓库或发布包分发。
仓库包含已打包的 assets/CraftTheSpire.pck，只修改 C# 逻辑时无需安装 Godot。修改游戏图片或本地化后，使用 Godot 4.5.1 运行 PackAssets.cmd，再运行 Build.cmd。只修改工坊介绍不需要重新打包游戏资源。
游戏更新导致 DLL 哈希变化时，构建脚本会停止。可用 CollectGameReferences.cmd 收集本地引用信息用于重新核对接口；收集结果不属于仓库或工坊发布内容。
开发与验证
内容模型通过 RitsuLib 注解注册。图鉴发现记录在模型、主菜单与当前档案就绪后写入；开局替换和卡牌权重调整使用 RitsuLib 管理的动态补丁，显式应用并检查结果。
随机矿物与事件沿用原生随机流，遗物状态通过原生保存属性接入。联机、读档、回滚及与其他模组组合的支持情况，以对应版本的实机测试记录与更新说明为准。
文档	内容
[游戏测试步骤](docs/TESTING.md)	加载、图鉴、新局木镐、卡牌、遗物、事件与联机边界
[验证记录](docs/VALIDATION.md)	已完成的接口与资源检查，以及仍待验证的项目
[实际 API 核对](docs/game-api-check.json)	目标程序集、调用签名和源码检查记录
[接口修正说明](docs/API-FIXES.md)	实际接口差异与接入方式
[内容 ID 清单](docs/content-index.json)	卡牌、遗物与事件的稳定 Entry ID


纯规则测试可单独执行：
dotnet run --project tests/CraftTheSpire.Tests.csproj --configuration Release
这些测试覆盖矿物分布、岩浆非致死生命损失、加权抽选分布与初始牌组替换规则。规则测试通过不代表全部游戏行为或联机同步已通过。
创意工坊发布与更新
项目提供 Mega Crit 官方上传器 使用的工作区模板。成功构建后，dist/CraftTheSpire-beta/ 包含 workshop.json、image.png 和待发布的 content/。
1. 编辑工作区的标题、介绍和可见性，保持 Steam 登录。
2. 运行 Upload.cmd，提供官方 ModUploader.exe 的完整路径。
3. 保留该工作区及生成的 mod_id.txt、workshop-identity.json，以后更新沿用同一条目。
维护已有工坊条目时，继续使用作者自己的原工作区与条目 ID。仓库模板默认私密，适合新建独立测试条目；公开发布流程使用 1.0.0 与对应构建的 release-checks.json。上传脚本会检查实际版本、测试记录与二进制哈希；具体步骤见 [发布说明](docs/RELEASE.md)。
项目文件
路径	用途
Code/Cards/	正式卡牌与临时矿物牌
Code/Relics/	四件遗物
Code/Events/	废弃矿井事件
Code/Utils/	矿物规则、图鉴初始化、开局与抽选接入
CraftTheSpire/images/	游戏图片资源
CraftTheSpire/localization/	zhs、eng 文本表
assets/	预打包 PCK 与资源哈希记录
tests/	独立于游戏程序集的纯规则测试
scripts/	构建、安装、资源打包与上传脚本
workshop/	工坊元数据模板与封面
docs/	测试、发布与 API 核对说明


发布源码时保留代码、资源、脚本、测试和文档。游戏引用文件、本机构建产物、日志与个人安装路径属于本地环境；已编译模组包可作为独立发布附件提供。工坊身份文件应在作者本地备份，其他开发者发布自己的派生模组时使用独立条目。
常见问题
图鉴为什么只有木镐和挖矿？
先选择无色牌，再清空搜索、稀有度、类型与费用筛选。木镐和挖矿是普通牌，铁镐与 TNT 是罕见牌，金苹果是稀有牌。五张正式牌应默认可见，四张临时矿物则按设计隐藏。
若仍缺少正式牌，核对当前加载的构建，并查看日志里的 Library initialization 和 Library card 记录。
开局为什么没有木镐？
替换只在生成新局初始牌组时发生，继续旧存档不会补发。确认模组已启用、安装的是当前构建，并查看日志中的 Card-access patches 应用结果。
木镐击杀敌人后为什么没有生成矿物？
若这一击结束了整场战斗，模组不再生成临时矿物。请在战斗仍继续的情况下测试挖矿效果。
上传时出现 k_EResultTimeout 怎么办？
Steam 的这个结果表示操作超时。保持 Steam 在线，保留已有工坊 ID 与身份记录，稍后使用同一工作区重试。若持续失败，提供上传器目录中的 mod-uploader.log。结果码定义见 Steamworks 文档。
反馈与贡献
欢迎通过仓库 Issues 提交问题、翻译修正或平衡建议。报告问题时，请附上游戏及 RitsuLib 版本、模组版本、单人或联机人数、相关角色与操作步骤，以及截图或日志。
- 构建问题：附上 build-log.txt 与第一条完整编译错误。
- 游戏问题：附上游戏日志中相关的错误片段。
- 上传问题：附上官方上传器生成的 mod-uploader.log。
修改随机、计数、出牌或保存逻辑时，请同时记录单人和联机验证结果；修改本地化或图片后重新打包 PCK。
许可与致谢
本项目新编写的代码、文档及生成美术按仓库 MIT License 发布；第三方游戏代码、依赖、商标和素材遵循各自许可。具体说明见 [第三方声明](THIRD-PARTY-NOTICES.txt)。
感谢 RitsuLib、Godot 与 Mega Crit 官方上传器 提供的基础设施。
本模组为非官方同人作品，与 Mojang、Microsoft 或 Mega Crit 无隶属或授权关系。图片采用原创生成美术，未打包《我的世界》原版纹理、音效或游戏代码。
English overview
Craft the Spire is a small Minecraft-inspired content mod for Slay the Spire 2 public-beta 0.111.0, requiring RitsuLib 0.6.2. It adds 5 collectible colorless cards, 4 temporary Mineral cards, 4 shared relics, and the Abandoned Mineshaft event.
Available on the Steam Workshop. Search for “我的世界小型内容包 Craft the Spire [Public Beta]” in the Slay the Spire 2 Workshop and subscribe together with RitsuLib. This repository contains the source code, artwork, localization and build scripts.
New runs replace one starting Strike with a Wooden Pickaxe. Mine resources during combat, turn coal into energy and iron into defense, and combine your finds with themed relics. The five collectible cards are revealed in the card library after initialization. Supports Simplified Chinese and English. See the test records and update notes for compatibility details.
Pick up your pickaxe. There might be a diamond under the next stone.
