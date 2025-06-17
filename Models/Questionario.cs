public class QuestionarioResposta
{
    public int QuestionarioRespostaId { get; set; }
    public int ClienteId { get; set; }
    public string? Pergunta { get; set; }
    public string? Resposta { get; set; }
}