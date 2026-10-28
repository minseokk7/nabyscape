using System;
using System.Collections.Generic;

namespace NabyScape;

internal static class LocalizationKoreanSharedCatalog
{
    internal static readonly IReadOnlyDictionary<string, string> Entries =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Exclude browser media sessions"] = "브라우저 미디어 감지 제외",
            ["Exclude browsers from music detection, the source picker, lyrics and dock media activity. Unknown hashed session identities are excluded too. Music apps with identifiable sessions remain available; browser playback is not stopped."] = "브라우저를 음악 감지, 재생 앱 목록, 가사 및 독의 미디어 활동에서 제외합니다. 해시 형태의 알 수 없는 세션도 제외됩니다. 식별 가능한 음악 앱은 계속 사용할 수 있으며 브라우저 재생을 중지하지 않습니다.",
            ["min"] = "분",
            ["+5 min"] = "+5분",
            ["Detach"] = "분리",
            ["Edit"] = "편집",
            ["Before installation, the updater saves a local recovery copy of the app and JSON profiles. If installation or initial startup fails, it restores that copy."] = "설치 전에 앱과 JSON 프로필의 복구 사본을 이 기기에 저장합니다. 설치 또는 최초 실행에 실패하면 이 사본으로 복원합니다.",
            ["Legacy installation: quit NabyScape and run the latest official setup, then open NabyScape.exe. Automatic installation and recovery require the current executable name."] = "이전 방식으로 설치된 앱입니다. NabyScape를 종료하고 최신 공식 설치 프로그램을 실행한 다음 NabyScape.exe를 여세요. 자동 설치 및 복구에는 현재 실행 파일 이름이 필요합니다.",
            ["Portable copy: use Release notes to download the portable ZIP, quit, then replace your portable files manually. Automatic installation is unavailable here."] = "포터블 버전입니다. 릴리스 노트에서 포터블 ZIP을 다운로드하고 앱을 종료한 다음 파일을 직접 교체하세요. 이 위치에서는 자동 설치를 사용할 수 없습니다.",
            ["No previous version has been saved by the updater yet."] = "업데이트 관리자가 저장한 이전 버전이 아직 없습니다.",
            ["Previous version: {0} · saved {1}"] = "이전 버전: {0} · 저장 시각: {1}",
            ["The saved recovery copy is unavailable or could not be verified."] = "저장된 복구 사본을 사용할 수 없거나 검증하지 못했습니다.",
            ["Language"] = "언어",
            ["Choose color · {0}"] = "색상 선택 · {0}",
            ["Color values · HEX / RGB"] = "색상 값 · HEX / RGB",
            ["Enter a six-digit color such as #182028"] = "#182028과 같은 6자리 색상 코드를 입력하세요",
            ["Drag to choose saturation and brightness. Arrow keys adjust; Shift makes larger changes."] = "드래그하여 채도와 밝기를 선택하세요. 방향키로 조절하고 Shift를 누르면 더 크게 조절합니다.",
            ["Red"] = "빨강",
            ["Green"] = "초록",
            ["Blue"] = "파랑",
            ["{0} saturation and brightness"] = "{0} 채도 및 밝기",
            ["{0} hue"] = "{0} 색조",
            ["{0} color picker"] = "{0} 색상 선택기",
            ["Choose {0} · color field, hue and optional HEX/RGB"] = "{0} 선택 · 색상 영역, 색조 및 HEX/RGB 값",
            ["Language · text size · contrast · Windows motion preferences"] = "언어 · 글자 크기 · 대비 · Windows 애니메이션 설정"
        };
}
