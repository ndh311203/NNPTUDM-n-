let adminChatConnection;
let currentUserId = null;
let currentUserInfo = null;

document.addEventListener('DOMContentLoaded', function () {
    const conversationsList = document.getElementById('conversationsList');
    const chatMessagesAdmin = document.getElementById('chatMessagesAdmin');
    const chatHeaderAdmin = document.getElementById('chatHeaderAdmin');
    const chatInputAreaAdmin = document.getElementById('chatInputAreaAdmin');
    const chatInputAdmin = document.getElementById('chatInputAdmin');
    const chatSendBtnAdmin = document.getElementById('chatSendBtnAdmin');
    const chatUserInfo = document.getElementById('chatUserInfo');

    chatSendBtnAdmin.addEventListener('click', sendMessage);
    chatInputAdmin.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            sendMessage();
        }
    });

    function loadConversations() {
        fetch('/Chat/GetConversations')
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    conversationsList.innerHTML = '';
                    if (data.conversations && data.conversations.length > 0) {
                        data.conversations.forEach(conv => {
                            const convDiv = document.createElement('div');
                            convDiv.className = 'card mb-2 conversation-item';
                            convDiv.style.cursor = 'pointer';
                            convDiv.dataset.userId = conv.userId;

                            const unreadBadge = conv.unreadCount > 0 
                                ? `<span class="badge bg-danger">${conv.unreadCount}</span>` 
                                : '';

                            convDiv.innerHTML = `
                                <div class="card-body">
                                    <div class="d-flex justify-content-between align-items-center">
                                        <div>
                                            <h6 class="mb-1">${escapeHtml(conv.hoTen || conv.email || 'Khách hàng')}</h6>
                                            <small class="text-muted">${new Date(conv.lastMessageTime).toLocaleString('vi-VN')}</small>
                                        </div>
                                        ${unreadBadge}
                                    </div>
                                </div>
                            `;

                            convDiv.addEventListener('click', function () {
                                selectConversation(conv.userId, conv.hoTen || conv.email || 'Khách hàng');
                            });

                            conversationsList.appendChild(convDiv);
                        });
                    } else {
                        conversationsList.innerHTML = '<div class="text-center text-muted"><small>Chưa có cuộc trò chuyện nào</small></div>';
                    }
                }
            })
            .catch(error => {
                console.error('Error loading conversations:', error);
            });
    }

    function selectConversation(userId, userName) {
        currentUserId = userId;
        currentUserInfo = userName;

        chatUserInfo.textContent = `Đang trò chuyện với: ${userName}`;
        chatHeaderAdmin.style.display = 'block';
        chatInputAreaAdmin.style.display = 'block';

        document.querySelectorAll('.conversation-item').forEach(item => {
            item.classList.remove('bg-light');
            if (item.dataset.userId == userId) {
                item.classList.add('bg-light');
            }
        });

        loadMessages(userId);
        markAsRead(userId);
    }

    function loadMessages(userId) {
        fetch(`/Chat/GetMessages?nguoiNhanId=${userId}&skip=0&take=100`)
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    chatMessagesAdmin.innerHTML = '';
                    if (data.messages && data.messages.length > 0) {
                        data.messages.forEach(msg => {
                            addMessageToUI(msg, msg.isMe);
                        });
                    } else {
                        chatMessagesAdmin.innerHTML = '<div class="text-center text-muted"><small>Chưa có tin nhắn nào</small></div>';
                    }
                    scrollToBottom();
                }
            })
            .catch(error => {
                console.error('Error loading messages:', error);
            });
    }

    function addMessageToUI(message, isMe) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `mb-3 ${isMe ? 'text-end' : 'text-start'}`;

        const messageBubble = document.createElement('div');
        messageBubble.className = 'd-inline-block p-3 rounded';
        messageBubble.style.cssText = `
            max-width: 70%;
            ${isMe ? 
                'background: linear-gradient(135deg, #FFE5E5 0%, #FFD6D6 100%); color: #654321;' : 
                'background: white; border: 1px solid #ADD8E6; color: #654321;'
            }
        `;

        const time = new Date(message.thoiGianGui).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
        messageBubble.innerHTML = `
            <div style="font-weight: 500; font-size: 0.9em; margin-bottom: 5px;">
                ${isMe ? 'Bạn' : (message.nguoiGuiTen || 'Khách hàng')}
            </div>
            <div>${escapeHtml(message.noiDung)}</div>
            <div style="font-size: 0.75em; color: #888; margin-top: 5px; text-align: right;">${time}</div>
        `;

        messageDiv.appendChild(messageBubble);
        chatMessagesAdmin.appendChild(messageDiv);
    }

    function sendMessage() {
        const message = chatInputAdmin.value.trim();
        if (!message || !currentUserId) return;

        fetch('/Chat/SendMessage', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: `nguoiNhanId=${currentUserId}&noiDung=${encodeURIComponent(message)}`
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                chatInputAdmin.value = '';
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

    function markAsRead(userId) {
        fetch('/Chat/MarkAsRead', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: `nguoiGuiId=${userId}`
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                loadConversations();
            }
        })
        .catch(error => {
            console.error('Error marking as read:', error);
        });
    }

    function scrollToBottom() {
        chatMessagesAdmin.scrollTop = chatMessagesAdmin.scrollHeight;
    }

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    function initSignalR() {
        adminChatConnection = new signalR.HubConnectionBuilder()
            .withUrl("/chatHub")
            .build();

        adminChatConnection.on("ReceiveMessage", function (message) {
            const messageUserId = parseInt(message.nguoiGuiId);
            const isMe = message.isAdmin || false;
            
            if (currentUserId && messageUserId === currentUserId && isMe) {
                addMessageToUI(message, true);
                scrollToBottom();
            } else if (!message.isAdmin && currentUserId === messageUserId) {
                addMessageToUI(message, false);
                scrollToBottom();
                markAsRead(messageUserId);
            } else if (!message.isAdmin) {
                loadConversations();
                if (currentUserId === messageUserId) {
                    addMessageToUI(message, false);
                    scrollToBottom();
                }
            }
        });

        adminChatConnection.start()
            .then(function () {
                console.log('Admin chat connected');
            })
            .catch(function (err) {
                console.error('SignalR connection error:', err);
            });

        adminChatConnection.onclose(function () {
            setTimeout(initSignalR, 5000);
        });
    }

    initSignalR();
    loadConversations();
    setInterval(loadConversations, 30000);
});

