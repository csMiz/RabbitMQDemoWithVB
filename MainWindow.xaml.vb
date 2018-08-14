
Imports System.Text
Imports RabbitMQ.Client
Imports RabbitMQ.Client.Events

Class MainWindow
    ''' <summary>
    ''' 使用RabbitMQ发送消息
    ''' </summary>
    Private Sub BtnClick(sender As Object, e As RoutedEventArgs)
        lbl1.Content = lbl1.Content & "正在连接..." & vbCrLf
        Dim factory As New ConnectionFactory
        Dim connection As IConnection = Nothing

        factory.Uri = New Uri("amqp://guest:guest@127.0.0.1:5672//")
        connection = factory.CreateConnection()

        Using connection
            Using channel As IModel = connection.CreateModel()
                channel.QueueDeclare("Topic.Q1", True, False, False, Nothing)

                Dim msg As String = "这是一条信息，来自.NET"

                For i = 0 To 9
                    channel.BasicPublish("", "Topic.Q1", Nothing, Encoding.UTF8.GetBytes(i.ToString & " : " & msg))
                Next

                lbl1.Content = lbl1.Content & "发送完成" & vbCrLf
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' 开始监听消息队列
    ''' </summary>
    Private Sub Btn2Click(sender As Object, e As RoutedEventArgs)
        Dim factory As ConnectionFactory = New ConnectionFactory()
        factory.Uri = New Uri("amqp://guest:guest@127.0.0.1:5672//")
        Dim connection As IConnection = factory.CreateConnection()
        Dim channel As IModel = connection.CreateModel()
        channel.QueueDeclare("Topic.Q1", True, False, False, Nothing)

        Dim consumer As EventingBasicConsumer = New EventingBasicConsumer(channel)
        AddHandler consumer.Received, AddressOf ConsumeQ1

        channel.BasicConsume("Topic.Q1", True, consumer)


    End Sub

    Private Sub ConsumeQ1(sender As Object, e As BasicDeliverEventArgs)
        Dim body() As Byte = e.Body
        Dim context As String = Encoding.UTF8.GetString(body)

        Dispatcher.BeginInvoke(New Action(Of String)(AddressOf postmsg), context)

    End Sub

    Private Sub postmsg(msg As String)
        Dim o As String = lbl1.Content
        lbl1.Content = o & msg & vbCrLf
    End Sub

End Class
