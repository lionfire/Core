using System;
using System.Linq;
using System.Collections.Generic;

namespace LionFire.Validation;

public class ValidationIssue
{

    public ValidationIssue() { }
    public ValidationIssue(string message) { this.Message = message; }
    public ValidationIssue(string message, object data) { this.Message = message; this.Data = data; }
    public ValidationIssue(object data)
    {
        if (data is string s) { Message = s; }
        else Data = data;
    }

    public static implicit operator ValidationIssue(string msg)
    {
        return new ValidationIssue(msg);
    }

    public string Key { get; set; }

    public object Kind { get; set; }

    /// <summary>
    /// Set for PropertyNotDefault, FieldNotSet, MemberNotSet, ArgumentNotSet
    /// </summary>
    public string VariableName { get; set; }

    public object Data { get; set; }

    public string Message
    {
        get { return message ?? Data?.ToString(); }
        set { message = value; }
    }
    private string message;

    public string Detail { get; set; }
    //public Dictionary<string, object> Properties { get; set; }

    public IEnumerable<ValidationIssue> InnerIssues { get; set; }

    public override string ToString()
    {
        return $"Key: {Key}, Kind: {Kind}, VariableName: {VariableName}, Message: {Message}, Detail: {Detail}, Data: {Data}, InnerIssues: {InnerIssues?.Count()}";
    }
}
