# 官方创意工坊发布

源码包不是上传工作区。只有本机 Build.cmd 成功后生成的 dist/CraftTheSpire-beta，才有可用的 DLL、PCK 和清单。官方上传器根目录要求 workshop.json、image.png，content 中只放本模组的三个发布文件。

先用 SetAuthor.cmd 填写作者，再 Build.cmd。完成本地基础测试后，默认使用 private 创建工坊条目。保持 Steam 登录，运行 Upload.cmd 并填写 ModUploader.exe 的实际完整路径。若上传器要求接受创意工坊协议，按它提供的 Steam 页面处理；本项目不代替用户上传。

首次得到 mod_id.txt 后保存整个工作区，包括上传脚本为新条目建立的 workshop-identity.json。Build.cmd 会更新 content，但保留这些文件和工坊说明。移动整个项目没有问题；不要把 SpireDraft 的 ID 搬来。上传脚本会拒绝没有 CraftTheSpire 身份记录的已有 ID，也会拒绝记录与 ID 不匹配的更新，避免发往其他条目。直接绕过 Upload.cmd 调用官方上传器不具备这项项目身份检查。预览图始终使用 workshop/image.png 作为构建来源，在工作区手动替换封面之后也应同步这个源文件。

私密条目建立后，在干净环境只订阅 RitsuLib 和本模组，完成启动和新局测试。订阅测试时先移走本地 mods/CraftTheSpire，避免重复加载。官方 minBranch/maxBranch 字段已经填写 public-beta；上传器模板提示这两个字段在 Steam 端可能有异常，应在网页再次核对。此版本不声明正式分支兼容。

首发按文档改成 1.0.0：同时修改 CraftTheSpire.csproj 的 Version 和根目录 CraftTheSpire.json 的 version，然后重新构建并验证这次二进制。修改同一工作区的 workshop.json 说明，删除“待测试”的表述只能以实际测试结果为依据。不要仅改文件夹名或标题假装切换版本。

把 docs/release-checks.template.json 复制到同一工作区并命名为 release-checks.json。按 TESTING.md 的实测结果填写，不通过的项保持 false。dllSha256、pckSha256 从该工作区 build-report.json 复制，测试必须针对这两个文件的当前哈希；游戏和 RitsuLib 实测版本也要填写。公共上传脚本会按文档的 Release Gate 检查这些字段，默认私密上传不要求整套发布门槛。

确认记录全部通过后，把同一工作区的 visibility 改为 public，再执行 Upload.cmd 更新该条目。保留 build-report、测试记录、mod_id.txt 及发布包 SHA256。首周优先处理崩溃、不同步和缺资源，不扩展文档以外的玩法。
