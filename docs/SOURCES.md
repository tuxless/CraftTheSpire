# 依据与接口边界

用户提供的 Craft the Spire 完整策划与技术方案 v0.1 是功能与数值依据。RitsuLib 0.6.2 NuGet 包及作者源码用于核对注册注解、模板、资源路径、保存属性接入及生成卡牌命令的调用形式。包版本固定，不使用 dev build 或通配符。

- RitsuLib： https://github.com/BAKAOLC/STS2-RitsuLib
- RitsuLib 内容注册： https://sts2-ritsulib.ritsukage.com/guide/content-authoring-toolkit
- RitsuLib 事件： https://sts2-ritsulib.ritsukage.com/guide/custom-events
- RitsuLib 0.6.2 包内仓库提交：1bebdb0365c34f8dc9066ded81ac4587df145449
- 官方上传器： https://github.com/megacrit/sts2-mod-uploader

工作区格式直接对照官方 template/workshop.json、template/README.md、src/ModConfig.cs 与 UploadCommand 的字段和参数。只有 content/ 中三个本模组文件发布到工坊。RitsuLib 工坊前置 ID 为 3747602295。

公开可查的游戏 API 参考用于理解原版卡牌、遗物、事件流程，不能代替用户 0.111.0 的 DLL。已发现公开示例和不同 API 分支之间存在方法差异，所以生成卡牌路径采用 RitsuLib 当前源码中的 Player 参数形式；最后必须用真实引用编译确认。工程不包含、也不发布任何反编译游戏源码、游戏 DLL 或框架 DLL。

资源包用官方 Godot 4.5.1 引擎在本次工作区导入和打包，并在干净项目中实际挂载验证。游戏 MegaDot 对相同纹理格式的加载、卡面裁切与语言排版，仍属于实机验收范围。
