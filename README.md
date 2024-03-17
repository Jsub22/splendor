## mini-project/splendor
- 날짜 : 2021.10~2021.11 (약 3주)
- 참여 인원 : 3인 (이정민, 정수빈, 최희주)
- 사용 스택 : C# Winform Socket, MySql

### [역할]
- 이정민 : Front-End(이미지 매핑, 이모티콘, 효과음)
- 정수빈 : Front-End(게임 화면), Back-End(게임 화면 게임 로직, 서버 파싱)
- 최희주 : Front-End(시작 화면 및 룸 화면), Back-End(유저 리스트 구성, DB 구성)

### [개발 동기]
최근 Splendor 게임을 오프라인으로 접하게 되어 온라인으로도 찾아 보았으나,
플레이어의 입장에서 도중에 통신이 되지 않는 오류 등 불편함이 존재했다.
그래서 통신 과목에서 배운 지식을 적용해 원활하게 통신할 수 있는 Splendor 게임을 만들어 보고 싶었다.
또한, 보드 게임 자체가 고려해야 할 경우의 수가 많아 개발 공부에 도움이 될 수 있을 것이라 생각되어 개발을 진행했다.

### [실행 과정 설명]
1. MultiChatServer.exe 실행 (Port 9000, 서버 IP 사용)
2. MultiChatClient.exe 실행 (Port 9000, Client IP 변경)
3. 1개의 서버와 N개의 클라이언트로 N : N 플레이

### [프로젝트 설명]
- 참조 사이트의 소스를 활용하여 콘솔 게임을 개발
- 버그 수정 및 예외 처리, 기능 개선 등을 처리

### [기능 흐름]
| |서버 구동 화면|
|------|---|
|서버 시작|![image](https://github.com/Jsub22/splendor_proj/assets/77329400/f9f7271d-5072-460c-8e5e-b54e740d339b)|

| |클라이언트 구동 화면 (플레이어 aa 시점)|
|------|---|
|닉네임 입력 (일회성)|![image](https://github.com/Jsub22/splendor_proj/assets/77329400/6840dfbb-93a0-47ef-94c3-c3fba477adea)|
|닉네임 변경|![image](https://github.com/Jsub22/splendor_proj/assets/77329400/971ed772-1bdb-4269-b4b9-6f7e29983176)|
|방 생성 또는 입장|![image](https://github.com/Jsub22/splendor_proj/assets/77329400/976c1f02-e042-4c40-a681-0f51c0f80e70)|
|규칙 설명|![image](https://github.com/Jsub22/splendor_proj/assets/77329400/174441a0-9271-4560-9a36-63a0a77ee34e)|
|플레이어 aa 방 생성|![image](https://github.com/Jsub22/splendor_proj/assets/77329400/101a751c-ebf2-47f0-90bc-e9d93b72094d)|

| |클라이언트 구동 화면 (플레이어 bb 시점)|
|------|---|
|플레이어 bb 입장 및 준비|![image](https://github.com/Jsub22/splendor_proj/assets/77329400/f4aa7989-93d5-48f4-a27f-f4c4f205a439)|
|랜덤 순서로 게임 시작|![image](https://github.com/Jsub22/splendor_proj/assets/77329400/cf6a7229-5748-4c9a-ae8c-c544b7fc49f3)|
|이모티콘|![image](https://github.com/Jsub22/splendor_proj/assets/77329400/450e849b-d366-48f4-b24c-0977e667e275)|

참조 사이트 : https://slaner.tistory.com/170?category=546117

### [개발 후기]
게임을 구현하면서 모든 경우의 수를 고려하기 쉽지 않았다.
토큰 게임이라 금세 로직이 복잡해지고 길어져서 클래스를 활용해 보고자 했으나 추가적으로 효율적인 알고리즘 또한 필요하다는 것을 느꼈다.
UI 또한 아쉬움이 많이 남는다.
