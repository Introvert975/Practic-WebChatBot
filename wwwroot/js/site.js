document.addEventListener('DOMContentLoaded', () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl('/chatHub')
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on('UpdateChat', updateMessageHistory);

    async function updateMessageHistory(newData) {
        const messageHistoryDiv = $('#History');
        messageHistoryDiv.empty(); // Очищаем содержимое элемента

        const username = getUsernameFromCookie(); // Получаем имя пользователя из cookie
        console.log('Текущий пользователь:', username);

        if (username && newData[username]) {
            console.log('Найденные данные для пользователя:', newData[username]);

            newData[username].forEach(message => {
                const splitMessage = message.quest.split('&/*DateMess*/->');
                const messageText = splitMessage[0];
                const messageDate = splitMessage[1] ? new Date(splitMessage[1]).toLocaleString() : new Date().toLocaleString();

                const messageDiv = $('<div/>').addClass('message-right').text(messageText);
                const dateSpan = $('<span/>').addClass('date').text(messageDate);
                messageDiv.append(dateSpan);
                messageHistoryDiv.append(messageDiv);

                const answerDiv = $('<div/>').addClass('message-left').html(message.answer);
                messageHistoryDiv.append(answerDiv);
            });
        } else {
            console.warn(username ? `Нет данных для пользователя ${username}` : 'Имя пользователя не найдено в cookie');
        }
    }

    function getUsernameFromCookie() {
        const name = 'username=';
        const decodedCookie = decodeURIComponent(document.cookie);
        const ca = decodedCookie.split(';');
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) === ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) === 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    }

    connection.start().catch(err => console.error('Ошибка подключения SignalR:', err));
});
