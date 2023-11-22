export interface IFeedback {
    feedbackId: number,
    userId: number,
    userFeedback: string,
    rating: number,
    postedAt: Date,
    adminReply: string,
    adminReplyTime: Date
}
