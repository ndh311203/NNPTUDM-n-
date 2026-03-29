let chatConnection;
let currentRecipientId = null;
let isAdmin = false;

document.addEventListener('DOMContentLoaded', function () {
    const chatToggleBtn = document.getElementById('chatToggleBtn');
    const chatBox = document.getElementById('chatBox');
    const chatMinimizeBtn = document.getElementById('chatMinimizeBtn');
    const chatSendBtn = document.getElementById('chatSendBtn');
    const chatInput = document.getElementById('chatInput');
    const chatMessages = document.getElementById('chatMessages');
    const chatUnreadBadge = document.getElementById('chatUnreadBadge');
    const chatStatus = document.getElementById('chatStatus');

    if (!chatToggleBtn || !chatBox) return;

    const bodyData = document.body.dataset;
    isAdmin = bodyData.isAdmin === 'true';
    const userId = bodyData.userId;

    if (!userId) return;

    chatToggleBtn.addEventListener('click', function () {
        if (chatBox.style.display === 'none' || chatBox.style.display === '') {
            chatBox.style.display = 'flex';
            chatMinimizeBtn.innerHTML = '<i class="bi bi-dash"></i>';
            loadMessages();
            updateUnreadCount();
        } else {
            chatBox.style.display = 'none';
        }
    });

    chatMinimizeBtn.addEventListener('click', function () {
        chatBox.style.display = 'none';
    });

    chatSendBtn.addEventListener('click', sendMessage);
    chatInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            sendMessage();
        }
    });

    function sendMessage() {
        const message = chatInput.value.trim();
        if (!message) return;

        const recipientId = isAdmin ? currentRecipientId : null;

        fetch('/Chat/SendMessage', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: `nguoiNhanId=${recipientId || ''}&noiDung=${encodeURIComponent(message)}`
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                chatInput.value = '';
                addMessageToUI(data.message, true);
                scrollToBottom();
            } else {
                alert('Lỗi: ' + (data.message || 'Không thể gửi tin nhắn'));
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Đã xảy ra lỗi khi gửi tin nhắn');
        });
    }

    function addMessageToUI(message, isMe) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `message ${isMe ? 'message-sent' : 'message-received'}`;
        messageDiv.style.cssText = `
            padding: 10px 15px;
            border-radius: 10px;
            max-width: 75%;
            word-wrap: break-word;
            ${isMe ? 
                'background: linear-gradient(135deg, #FFE5E5 0%, #FFD6D6 100%); margin-left: auto; color: #654321;' : 
                'background: white; border: 1px solid #ADD8E6; color: #654321;'
            }
        `;

        const time = new Date(message.thoiGianGui).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
        messageDiv.innerHTML = `
            <div style="font-weight: 500; font-size: 0.85em; margin-bottom: 5px;">
                ${isMe ? 'Bạn' : (message.nguoiGuiTen || 'Hỗ trợ viên')}
            </div>
            <div>${escapeHtml(message.noiDung)}</div>
            <div style="font-size: 0.75em; color: #888; margin-top: 5px; text-align: right;">${time}</div>
        `;

        const emptyDiv = chatMessages.querySelector('.text-center.text-muted');
        if (emptyDiv) {
            emptyDiv.remove();
        }

        chatMessages.appendChild(messageDiv);
    }

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    function scrollToBottom() {
        const chatBody = document.getElementById('chatBody');
        if (chatBody) {
            chatBody.scrollTop = chatBody.scrollHeight;
        }
    }

    function loadMessages() {
        const recipientId = isAdmin ? currentRecipientId : null;
        const url = `/Chat/GetMessages?nguoiNhanId=${recipientId || ''}&skip=0&take=50`;

        fetch(url)
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    chatMessages.innerHTML = '';
                    if (data.messages && data.messages.length > 0) {
                        data.messages.forEach(msg => {
                            addMessageToUI(msg, msg.isMe);
                        });
                    } else {
                        chatMessages.innerHTML = '<div class="text-center text-muted"><small>Chưa có tin nhắn nào. Hãy bắt đầu cuộc trò chuyện!</small></div>';
                    }
                    scrollToBottom();
                }
            })
            .catch(error => {
                console.error('Error loading messages:', error);
            });
    }

    function updateUnreadCount() {
        fetch('/Chat/GetUnreadCount')
            .then(response => response.json())
            .then(data => {
                if (data.success && data.count > 0) {
                    chatUnreadBadge.textContent = data.count;
                    chatUnreadBadge.style.display = 'block';
                } else {
                    chatUnreadBadge.style.display = 'none';
                }
            })
            .catch(error => {
                console.error('Error updating unread count:', error);
            });
    }

    function initSignalR() {
        chatConnection = new signalR.HubConnectionBuilder()
            .withUrl("/chatHub")
            .build();

        chatConnection.on("ReceiveMessage", function (message) {
            const isMe = parseInt(message.nguoiGuiId) === parseInt(userId);
            
            if (isMe || (!isAdmin && message.isAdmin) || (isAdmin && currentRecipientId && parseInt(message.nguoiGuiId) === currentRecipientId)) {
                addMessageToUI(message, isMe);
                scrollToBottom();
                
                if (!isMe && chatBox.style.display === 'none') {
                    updateUnreadCount();
                }
            }
        });

        chatConnection.start()
            .then(function () {
                chatStatus.textContent = 'Đã kết nối';
                chatStatus.style.color = '#28a745';
                updateUnreadCount();
                setInterval(updateUnreadCount, 30000);
            })
            .catch(function (err) {
                console.error('SignalR connection error:', err);
                chatStatus.textContent = 'Mất kết nối';
                chatStatus.style.color = '#dc3545';
            });

        chatConnection.onclose(function () {
            chatStatus.textContent = 'Mất kết nối';
            chatStatus.style.color = '#dc3545';
            setTimeout(initSignalR, 5000);
        });
    }

    initSignalR();
    updateUnreadCount();
    setInterval(updateUnreadCount, 60000);
});

