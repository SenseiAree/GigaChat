export interface IMessage {
        messageId: number,
        chatId: number,
        senderId: number,
        receiverId: number,
        content: string,
        sentTime: Date,
        isRead: string
}
